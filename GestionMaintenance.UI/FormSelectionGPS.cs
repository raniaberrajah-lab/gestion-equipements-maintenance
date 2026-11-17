using System;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace GestionMaintenance.UI
{
    public class FormSelectionGPS : Form
    {
        private readonly WebView2 navigateur = new();
        private readonly Label lblPosition = new();
        private readonly Button btnUtiliser = new();

        public string? AdresseSelectionnee { get; private set; }

        public FormSelectionGPS()
        {
            Text = "Choisir une localisation sur la carte";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;

            lblPosition.Text =
                "Cliquez sur la carte pour récupérer une adresse.";

            lblPosition.Dock = DockStyle.Top;
            lblPosition.Height = 45;
            lblPosition.TextAlign = ContentAlignment.MiddleCenter;

            btnUtiliser.Text = "Utiliser cette adresse";
            btnUtiliser.Dock = DockStyle.Bottom;
            btnUtiliser.Height = 45;
            btnUtiliser.Enabled = false;
            btnUtiliser.Click += BtnUtiliser_Click;

            navigateur.Dock = DockStyle.Fill;

            Controls.Add(navigateur);
            Controls.Add(lblPosition);
            Controls.Add(btnUtiliser);

            Shown += FormSelectionGPS_Shown;
        }

        private async void FormSelectionGPS_Shown(
            object? sender, EventArgs e)
        {
            try
            {
                await navigateur.EnsureCoreWebView2Async();

                navigateur.CoreWebView2.WebMessageReceived +=
                    Navigateur_WebMessageReceived;

                navigateur.NavigateToString("""
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport"
          content="width=device-width, initial-scale=1.0">

    <link rel="stylesheet"
          href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css">

    <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js">
    </script>

    <style>
        html, body, #map {
            height: 100%;
            width: 100%;
            margin: 0;
            padding: 0;
        }
    </style>
</head>

<body>
    <div id="map"></div>

    <script>
        var map = L.map('map').setView([36.8065, 10.1815], 12);

        L.tileLayer(
            'https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}',
            {
                attribution: 'Tiles © Esri'
            }
        ).addTo(map);

        var marqueur = null;
        var requeteEnCours = false;

        map.on('click', async function(e) {

            if (requeteEnCours) return;

            requeteEnCours = true;

            var latitude = e.latlng.lat;
            var longitude = e.latlng.lng;

            if (marqueur !== null) {
                map.removeLayer(marqueur);
            }

            marqueur = L.marker([latitude, longitude]).addTo(map);

            window.chrome.webview.postMessage({
                type: "loading"
            });

            try {
                var url =
                    "https://nominatim.openstreetmap.org/reverse" +
                    "?format=jsonv2" +
                    "&lat=" + latitude +
                    "&lon=" + longitude +
                    "&accept-language=fr";

                var response = await fetch(url, {
                    headers: {
                        "Accept": "application/json"
                    }
                });

                if (!response.ok) {
                    throw new Error("Service d'adresse indisponible.");
                }

                var data = await response.json();

                if (!data.display_name) {
                    throw new Error("Aucune adresse trouvée.");
                }

                window.chrome.webview.postMessage({
                    type: "address",
                    address: data.display_name
                });

            } catch (error) {

                window.chrome.webview.postMessage({
                    type: "error",
                    message: error.message
                });

            } finally {
                requeteEnCours = false;
            }
        });
    </script>
</body>
</html>
""");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Impossible de charger la carte :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Navigateur_WebMessageReceived(
            object? sender,
            CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                using JsonDocument document =
                    JsonDocument.Parse(e.WebMessageAsJson);

                JsonElement racine = document.RootElement;

                string? type = racine.GetProperty("type").GetString();

                if (type == "loading")
                {
                    AdresseSelectionnee = null;
                    btnUtiliser.Enabled = false;
                    lblPosition.Text = "Recherche de l'adresse en cours...";
                }
                else if (type == "address")
                {
                    AdresseSelectionnee =
                        racine.GetProperty("address").GetString();

                    lblPosition.Text = AdresseSelectionnee;
                    btnUtiliser.Enabled = true;
                }
                else if (type == "error")
                {
                    AdresseSelectionnee = null;
                    btnUtiliser.Enabled = false;

                    string message =
                        racine.GetProperty("message").GetString()
                        ?? "Impossible de récupérer l'adresse.";

                    lblPosition.Text = message;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur de lecture de l'adresse : " + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnUtiliser_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AdresseSelectionnee))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
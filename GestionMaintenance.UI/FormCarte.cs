using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using GestionMaintenance;

namespace GestionMaintenance.UI
{
    public class FormCarte : Form
    {
        private readonly WebView2 navigateur = new WebView2();

        public FormCarte(List<Equipement> equipements)
        {
            Text = "Carte des équipements";
            Width = 1100;
            Height = 750;
            StartPosition = FormStartPosition.CenterScreen;

            navigateur.Dock = DockStyle.Fill;
            Controls.Add(navigateur);

            Load += async (sender, e) =>
            {
                try
                {
                    await navigateur.EnsureCoreWebView2Async();

                    string html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>

    <title>Carte des équipements</title>

    <style>
        body {
            font-family: Arial, sans-serif;
            background: #f1f5f9;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }

        .message {
            background: white;
            padding: 35px;
            border-radius: 12px;
            text-align: center;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            max-width: 500px;
        }

        h2 {
            color: #2563eb;
        }

        p {
            color: #475569;
            line-height: 1.6;
        }
    </style>
</head>

<body>
    <div class='message'>
        <h2>Carte des équipements</h2>
        <p>
            Les coordonnées GPS ne sont plus enregistrées
            dans l'application.
        </p>
        <p>
            Les équipements conservent leur adresse textuelle
            dans le champ Localisation.
        </p>
    </div>
</body>
</html>";

                    navigateur.NavigateToString(html);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Erreur lors du chargement de la carte :\n" + ex.Message,
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            };
        }
    }
}
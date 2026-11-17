using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GestionMaintenance;
using GestionMaintenance.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using Color = System.Drawing.Color;
using Size = System.Drawing.Size;

namespace GestionMaintenance.UI
{
    public partial class Form1 : Form
    {
        private readonly string cheminBD = ObtenirCheminBase();

        private readonly Panel contenu = new();
        private readonly DataGridView tableau = new();
        private readonly TextBox recherche = new();

        private List<Equipement> listeEquipements = new();

        private readonly Color fond = Color.FromArgb(245, 247, 250);
        private readonly Color bleu = Color.FromArgb(37, 99, 235);
        private readonly Color sombre = Color.FromArgb(24, 35, 55);

        private static string ObtenirCheminBase()
        {
            string dossierProjet = Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..", "..", "..", ".."));

            return Path.Combine(dossierProjet, "gestion-maintenance.db");
        }

        public Form1()
        {
            InitializeComponent();

            Text = "MaintenancePro - Gestion de maintenance";
            Size = new Size(1250, 750);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = fond;
            Font = new Font("Segoe UI", 10);

            CreerInterface();

            try
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(cheminBD)!);

                using var db = CreerContexte();
                db.Database.Migrate();

                ChargerEquipements();
                AfficherDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur d'initialisation :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= BASE DE DONNEES =================

        private AppDbContext CreerContexte()
        {
            var options =
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite($"Data Source={cheminBD}")
                    .Options;

            return new AppDbContext(options);
        }

        private void ChargerEquipements()
        {
            using var db = CreerContexte();

            listeEquipements = db.Equipements
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .ToList();
        }

        // ================= INTERFACE PRINCIPALE =================

        private void CreerInterface()
        {
            Panel sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 225,
                BackColor = sombre
            };

            Label logo = new Label
            {
                Text = "MAINTENANCE\nPRO",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(22, 25),
                Size = new Size(185, 65)
            };

            sidebar.Controls.Add(logo);

            AjouterNavigation(sidebar, "Dashboard", 125, AfficherDashboard);
            AjouterNavigation(sidebar, "Équipements", 180, AfficherEquipements);

            AjouterNavigation(sidebar, "Techniciens", 235,
                () => AfficherModule("Techniciens"));

            AjouterNavigation(sidebar, "Pièces détachées", 290,
                () => AfficherModule("Pièces détachées"));

            AjouterNavigation(sidebar, "Interventions", 345,
                () => AfficherModule("Interventions"));

            Label pied = new Label
            {
                Text = "Gestion de maintenance\nVersion 1.0",
                ForeColor = Color.Silver,
                Location = new Point(20, 650),
                Size = new Size(190, 45)
            };

            sidebar.Controls.Add(pied);

            contenu.Dock = DockStyle.Fill;
            contenu.BackColor = fond;

            Controls.Add(contenu);
            Controls.Add(sidebar);
        }

        private void AjouterNavigation(
            Panel sidebar,
            string texte,
            int y,
            Action action)
        {
            Button bouton = new Button
            {
                Text = "   " + texte,
                Location = new Point(12, y),
                Size = new Size(200, 45),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = sombre,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };

            bouton.FlatAppearance.BorderSize = 0;
            bouton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(48, 65, 95);

            bouton.Click += (s, e) => action();

            sidebar.Controls.Add(bouton);
        }

        private Label CreerTitre(string texte)
        {
            return new Label
            {
                Text = texte,
                Font = new Font("Segoe UI", 21, FontStyle.Bold),
                ForeColor = sombre,
                Location = new Point(30, 25),
                AutoSize = true
            };
        }

        private void AfficherModule(string nom)
        {
            contenu.Controls.Clear();
            contenu.Controls.Add(CreerTitre(nom));

            Label message = new Label
            {
                Text = "Le module " + nom +
                    " sera développé dans la prochaine étape.",
                Location = new Point(35, 100),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 12)
            };

            contenu.Controls.Add(message);
        }

        // ================= DASHBOARD =================

        private void AfficherDashboard()
        {
            contenu.Controls.Clear();
            contenu.Controls.Add(CreerTitre("Tableau de bord"));

            Label sousTitre = new Label
            {
                Text = "Vue générale de votre activité",
                Location = new Point(35, 70),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            contenu.Controls.Add(sousTitre);

            int total = listeEquipements.Count;

            int enService = listeEquipements.Count(
                e => e.Statut == StatutEquipement.EnService);

            int enPanne = listeEquipements.Count(
                e => e.Statut == StatutEquipement.EnPanne);

            int enMaintenance = listeEquipements.Count(
                e => e.Statut == StatutEquipement.EnMaintenance);

            int horsService = listeEquipements.Count(
                e => e.Statut == StatutEquipement.HorsService);

            AjouterCarte("Total équipements", total.ToString(),
                Color.FromArgb(37, 99, 235), 35);

            AjouterCarte("En service", enService.ToString(),
                Color.FromArgb(22, 163, 74), 290);

            AjouterCarte("En panne", enPanne.ToString(),
                Color.FromArgb(220, 38, 38), 545);

            AjouterCarte("En maintenance", enMaintenance.ToString(),
                Color.FromArgb(217, 119, 6), 800);

            AjouterCarte("Hors service", horsService.ToString(),
                Color.FromArgb(107, 114, 128), 35, 270);

            Button voir = new Button
            {
                Text = "Consulter les équipements",
                Location = new Point(35, 410),
                Size = new Size(240, 45),
                BackColor = bleu,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            voir.FlatAppearance.BorderSize = 0;
            voir.Click += (s, e) => AfficherEquipements();

            contenu.Controls.Add(voir);
        }

        private void AjouterCarte(
            string titre,
            string valeur,
            Color couleur,
            int x,
            int y = 125)
        {
            Panel carte = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(220, 105),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitre = new Label
            {
                Text = titre,
                Location = new Point(15, 15),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            Label lblValeur = new Label
            {
                Text = valeur,
                Location = new Point(15, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                ForeColor = couleur
            };

            carte.Controls.Add(lblTitre);
            carte.Controls.Add(lblValeur);

            contenu.Controls.Add(carte);
        }

        // ================= EQUIPEMENTS =================

        private void AfficherEquipements()
        {
            contenu.Controls.Clear();
            contenu.Controls.Add(CreerTitre("Gestion des équipements"));

            Label sousTitre = new Label
            {
                Text = "Consulter et gérer les équipements enregistrés",
                Location = new Point(35, 70),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            contenu.Controls.Add(sousTitre);

            recherche.Text = "";
            recherche.PlaceholderText = "Rechercher un équipement...";
            recherche.Location = new Point(35, 115);
            recherche.Size = new Size(300, 38);
            recherche.Font = new Font("Segoe UI", 10);

            recherche.TextChanged -= Recherche_TextChanged;
            recherche.TextChanged += Recherche_TextChanged;

            contenu.Controls.Add(recherche);

            Button ajouter = CreerBouton(
                "Ajouter", 355, Color.FromArgb(22, 163, 74));

            Button modifier = CreerBouton("Modifier", 485, bleu);

            Button supprimer = CreerBouton(
                "Supprimer", 615, Color.FromArgb(220, 38, 38));

            Button exporter = CreerBouton(
                "Exporter PDF", 745, Color.FromArgb(185, 28, 28));

            ajouter.Click += BtnAjouter_Click;
            modifier.Click += BtnModifier_Click;
            supprimer.Click += BtnSupprimer_Click;
            exporter.Click += BtnExporterPdf_Click;

            contenu.Controls.Add(ajouter);
            contenu.Controls.Add(modifier);
            contenu.Controls.Add(supprimer);
            contenu.Controls.Add(exporter);

            tableau.Location = new Point(35, 175);

            tableau.Size = new Size(
                Math.Max(700, contenu.ClientSize.Width - 70),
                Math.Max(300, contenu.ClientSize.Height - 210));

            tableau.Anchor = AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            tableau.BackgroundColor = Color.White;
            tableau.BorderStyle = BorderStyle.None;
            tableau.ReadOnly = true;
            tableau.MultiSelect = false;
            tableau.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            tableau.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            tableau.AllowUserToAddRows = false;
            tableau.RowHeadersVisible = false;
            tableau.AutoGenerateColumns = true;
            tableau.EnableHeadersVisualStyles = false;

            tableau.ColumnHeadersDefaultCellStyle.BackColor = sombre;
            tableau.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tableau.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            tableau.ColumnHeadersHeight = 42;
            tableau.RowTemplate.Height = 35;

            tableau.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(219, 234, 254);

            tableau.DefaultCellStyle.SelectionForeColor = Color.Black;

            contenu.Controls.Add(tableau);

            ActualiserTableau(listeEquipements);
        }

        private Button CreerBouton(
            string texte,
            int x,
            Color couleur)
        {
            Button bouton = new Button
            {
                Text = texte,
                Location = new Point(x, 113),
                Size = new Size(115, 40),
                BackColor = couleur,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            bouton.FlatAppearance.BorderSize = 0;
            return bouton;
        }

        private void ActualiserTableau(List<Equipement> donnees)
        {
            tableau.DataSource = null;
            tableau.DataSource = donnees;

            if (tableau.Columns.Contains("Interventions"))
                tableau.Columns["Interventions"].Visible = false;

            if (tableau.Columns.Contains("Id"))
                tableau.Columns["Id"].HeaderText = "ID";

            if (tableau.Columns.Contains("Reference"))
                tableau.Columns["Reference"].HeaderText = "Référence";

            if (tableau.Columns.Contains("Designation"))
                tableau.Columns["Designation"].HeaderText = "Désignation";

            if (tableau.Columns.Contains("Marque"))
                tableau.Columns["Marque"].HeaderText = "Marque";

            if (tableau.Columns.Contains("Localisation"))
                tableau.Columns["Localisation"].HeaderText = "Localisation";

            if (tableau.Columns.Contains("DateAcquisition"))
                tableau.Columns["DateAcquisition"].HeaderText = "Date acquisition";

            if (tableau.Columns.Contains("CoutAcquisition"))
                tableau.Columns["CoutAcquisition"].HeaderText = "Coût";

            if (tableau.Columns.Contains("SpecialiteRequise"))
                tableau.Columns["SpecialiteRequise"].HeaderText = "Spécialité";

            if (tableau.Columns.Contains("Statut"))
                tableau.Columns["Statut"].HeaderText = "Statut";
        }

        private void Recherche_TextChanged(object? sender, EventArgs e)
        {
            string terme = recherche.Text.Trim();

            var resultat = listeEquipements
                .Where(x =>
                    (x.Reference ?? "").Contains(
                        terme, StringComparison.OrdinalIgnoreCase) ||

                    (x.Designation ?? "").Contains(
                        terme, StringComparison.OrdinalIgnoreCase) ||

                    (x.Marque ?? "").Contains(
                        terme, StringComparison.OrdinalIgnoreCase) ||

                    (x.Localisation ?? "").Contains(
                        terme, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ActualiserTableau(resultat);
        }

        private int? ObtenirIdSelectionne()
        {
            if (tableau.CurrentRow?.DataBoundItem is Equipement equipement)
                return equipement.Id;

            return null;
        }

        // ================= CHANGEMENT DU STATUT =================

        private void AppliquerStatut(
            Equipement equipement,
            StatutEquipement nouveauStatut)
        {
            if (equipement.Statut == nouveauStatut)
                return;

            switch (nouveauStatut)
            {
                case StatutEquipement.EnService:
                    equipement.RemettreEnService();
                    break;

                case StatutEquipement.EnPanne:
                    equipement.MettreEnPanne();
                    break;

                case StatutEquipement.EnMaintenance:
                    equipement.MettreEnMaintenance();
                    break;

                case StatutEquipement.HorsService:
                    equipement.MettreHorsService();
                    break;
            }
        }

        // ================= AJOUTER =================

        private void BtnAjouter_Click(object? sender, EventArgs e)
        {
            using var formulaire = new FormEquipement();

            if (formulaire.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                using var db = CreerContexte();

                var nouvelEquipement = formulaire.EquipementCree!;

                AppliquerStatut(nouvelEquipement, formulaire.StatutSelectionne);

                db.Equipements.Add(nouvelEquipement);
                db.SaveChanges();

                ChargerEquipements();
                AfficherEquipements();

                MessageBox.Show(
                    "Équipement ajouté avec succès !",
                    "Succès",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de l'ajout :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= MODIFIER =================

        private void BtnModifier_Click(object? sender, EventArgs e)
        {
            int? id = ObtenirIdSelectionne();

            if (id == null)
            {
                MessageBox.Show("Sélectionnez un équipement.");
                return;
            }

            try
            {
                using var db = CreerContexte();

                var equipement = db.Equipements
                    .FirstOrDefault(x => x.Id == id.Value);

                if (equipement == null)
                {
                    MessageBox.Show("Équipement introuvable.");
                    ChargerEquipements();
                    AfficherEquipements();
                    return;
                }

                using var formulaire = new FormEquipement(equipement);

                if (formulaire.ShowDialog(this) != DialogResult.OK)
                    return;

                equipement.Designation =
                    formulaire.EquipementCree!.Designation;

                equipement.Marque =
                    formulaire.EquipementCree.Marque;

                equipement.Localisation =
                    formulaire.EquipementCree.Localisation;

                AppliquerStatut(equipement, formulaire.StatutSelectionne);

                db.SaveChanges();

                ChargerEquipements();
                AfficherEquipements();

                MessageBox.Show(
                    "Équipement modifié avec succès !",
                    "Succès",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la modification :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= SUPPRIMER =================

        private void BtnSupprimer_Click(object? sender, EventArgs e)
        {
            int? id = ObtenirIdSelectionne();

            if (id == null)
            {
                MessageBox.Show("Sélectionnez un équipement.");
                return;
            }

            DialogResult confirmation = MessageBox.Show(
                "Voulez-vous vraiment supprimer cet équipement ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation != DialogResult.Yes)
                return;

            try
            {
                using var db = CreerContexte();

                var equipement = db.Equipements
                    .FirstOrDefault(x => x.Id == id.Value);

                if (equipement == null)
                {
                    MessageBox.Show("Équipement introuvable.");
                    ChargerEquipements();
                    AfficherEquipements();
                    return;
                }

                db.Equipements.Remove(equipement);
                db.SaveChanges();

                ChargerEquipements();
                AfficherEquipements();

                MessageBox.Show(
                    "Équipement supprimé avec succès !",
                    "Succès",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la suppression :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= EXPORTER PDF =================

        private void BtnExporterPdf_Click(object? sender, EventArgs e)
        {
            var equipements = tableau.Rows
                .Cast<DataGridViewRow>()
                .Where(row => row.DataBoundItem is Equipement)
                .Select(row => (Equipement)row.DataBoundItem!)
                .ToList();

            if (equipements.Count == 0)
            {
                MessageBox.Show(
                    "Aucun équipement à exporter.",
                    "Export PDF",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using SaveFileDialog dialogue = new SaveFileDialog
            {
                Title = "Enregistrer le rapport PDF",
                Filter = "Fichier PDF (.pdf)|.pdf",
                DefaultExt = "pdf",
                AddExtension = true,
                FileName = $"Rapport_Equipements_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };

            if (dialogue.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                QuestPDF.Settings.UseSystemFonts = true;

                Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(25);

                        page.DefaultTextStyle(x => x
                            .FontFamily("Lato", "Arial")
                            .FontSize(8));

                        // En-tête du rapport
                        page.Header().Column(column =>
                        {
                            column.Item()
                                .Text("MAINTENANCE PRO")
                                .FontSize(20)
                                .Bold()
                                .FontColor("#263746");

                            column.Item()
                                .Text("Rapport des équipements")
                                .FontSize(13)
                                .FontColor("#2563EB");

                            column.Item()
                                .Text($"Date du rapport : {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9);

                            column.Item()
                                .Text($"Nombre total d'équipements : {equipements.Count}")
                                .FontSize(9)
                                .Bold();

                            column.Item()
                                .PaddingVertical(5)
                                .LineHorizontal(1)
                                .LineColor("#D1D5DB");
                        });

                        // Tableau des équipements
                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(0.9f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.2f);
                            });

                            table.Header(header =>
                            {
                                string[] titres =
                                {
                                    "Référence",
                                    "Désignation",
                                    "Marque",
                                    "Localisation",
                                    "Date acquisition",
                                    "Coût",
                                    "Spécialité",
                                    "Statut"
                                };

                                foreach (string titre in titres)
                                {
                                    header.Cell()
                                        .Element(CelluleEntete)
                                        .Text(titre);
                                }
                            });

                            foreach (var equipement in equipements)
                            {
                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatValeur(equipement.Reference));

                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatValeur(equipement.Designation));

                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatValeur(equipement.Marque));

                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatValeur(equipement.Localisation));

                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatDate(equipement.DateAcquisition));

                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatCout(equipement.CoutAcquisition));

                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatValeur(equipement.SpecialiteRequise));

                                table.Cell().Element(CelluleCorps)
                                    .Text(FormatValeur(equipement.Statut));
                            }
                        });

                        // Pied de page
                        page.Footer()
                            .AlignRight()
                            .Text(text =>
                            {
                                text.Span("Page ");
                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                    });
                }).GeneratePdf(dialogue.FileName);

                // Vérifier que le PDF a bien été créé
                if (!File.Exists(dialogue.FileName))
                {
                    MessageBox.Show(
                        "Le fichier PDF n'a pas été trouvé après sa génération.",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                // Confirmer la création
                MessageBox.Show(
                    "Le rapport PDF a été créé avec succès !\n\nEmplacement :\n"
                    + dialogue.FileName,
                    "Export réussi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Ouvrir le PDF avec l'application associée à Windows
                try
                {
                    System.Diagnostics.Process.Start(
                        "explorer.exe",
                        "\"" + dialogue.FileName + "\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Le PDF existe, mais son ouverture automatique a échoué.\n\n"
                        + "Vous pouvez l'ouvrir manuellement depuis :\n"
                        + dialogue.FileName
                        + "\n\nDétail : " + ex.Message,
                        "Ouverture du PDF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la génération du PDF :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= CONVERSIONS POUR LE PDF =================

        private static string FormatValeur(object? valeur)
        {
            return valeur?.ToString() ?? "";
        }

        private static string FormatDate(object? valeur)
        {
            if (valeur == null)
                return "";

            if (DateTime.TryParse(valeur.ToString(), out DateTime date))
                return date.ToString("dd/MM/yyyy");

            return valeur.ToString() ?? "";
        }

        private static string FormatCout(object? valeur)
        {
            if (valeur == null)
                return "";

            if (decimal.TryParse(
                valeur.ToString(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal cout))
            {
                return cout.ToString(
                    "N2",
                    System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
            }

            return valeur.ToString() ?? "";
        }

        // ================= STYLE DU TABLEAU PDF =================

        private static IContainer CelluleEntete(IContainer container)
        {
            return container
                .Background("#263746")
                .Padding(5)
                .DefaultTextStyle(x =>
                    x.FontColor(Colors.White).Bold());
        }

        private static IContainer CelluleCorps(IContainer container)
        {
            return container
                .BorderBottom(0.5f)
                .BorderColor("#D1D5DB")
                .Padding(5);
        }
    }
}
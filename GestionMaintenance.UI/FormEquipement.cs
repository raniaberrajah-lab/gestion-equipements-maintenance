using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GestionMaintenance.UI
{
    public class FormEquipement : Form
    {
        private readonly TextBox txtReference = new();
        private readonly TextBox txtDesignation = new();
        private readonly TextBox txtMarque = new();
        private readonly TextBox txtLocalisation = new();

        private readonly DateTimePicker dateAcquisition = new();
        private readonly NumericUpDown numCout = new();
        private readonly ComboBox cmbSpecialite = new();
        private readonly ComboBox cmbStatut = new();

        private readonly ErrorProvider errorProvider = new();
        private readonly Equipement? equipementOriginal;

        public Equipement? EquipementCree { get; private set; }

        public StatutEquipement StatutSelectionne =>
            (StatutEquipement)cmbStatut.SelectedItem!;

        // Constructeur pour ajouter un équipement
        public FormEquipement()
        {
            InitialiserInterface();
        }

        // Constructeur pour modifier un équipement
        public FormEquipement(Equipement equipement)
        {
            equipementOriginal = equipement;

            InitialiserInterface();

            txtReference.Text = equipement.Reference;
            txtDesignation.Text = equipement.Designation;
            txtMarque.Text = equipement.Marque;
            txtLocalisation.Text = equipement.Localisation;

            dateAcquisition.Value = equipement.DateAcquisition;

            numCout.Value = Math.Max(
                numCout.Minimum,
                Math.Min(numCout.Maximum, equipement.CoutAcquisition)
            );

            cmbSpecialite.SelectedItem = equipement.SpecialiteRequise;
            cmbStatut.SelectedItem = equipement.Statut;

            // Ces informations ne doivent pas être modifiées
            txtReference.ReadOnly = true;
            dateAcquisition.Enabled = false;
            numCout.Enabled = false;
            cmbSpecialite.Enabled = false;
        }

        // Ajouter un label et son contrôle à la fenêtre
        private void AjouterChamp(string nom, Control controle, int y)
        {
            Label label = new Label
            {
                Text = nom,
                Location = new Point(25, y),
                Size = new Size(140, 25)
            };

            controle.Location = new Point(175, y);
            controle.Size = new Size(230, 28);

            Controls.Add(label);
            Controls.Add(controle);
        }

        // Initialiser l'interface graphique
        private void InitialiserInterface()
        {
            Text = equipementOriginal == null
                ? "Ajouter un équipement"
                : "Modifier un équipement";

            Size = new Size(470, 570);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            AutoValidate = AutoValidate.EnablePreventFocusChange;

            AjouterChamp("Référence :", txtReference, 25);
            AjouterChamp("Désignation :", txtDesignation, 70);
            AjouterChamp("Marque :", txtMarque, 115);
            AjouterChamp("Localisation :", txtLocalisation, 160);
            AjouterChamp("Date acquisition :", dateAcquisition, 205);
            AjouterChamp("Coût acquisition :", numCout, 250);
            AjouterChamp("Spécialité :", cmbSpecialite, 295);
            AjouterChamp("Statut :", cmbStatut, 340);

            // Longueur maximale des champs
            txtReference.MaxLength = 20;
            txtDesignation.MaxLength = 100;
            txtMarque.MaxLength = 50;
            txtLocalisation.MaxLength = 100;

            // Configuration du coût
            numCout.DecimalPlaces = 2;
            numCout.Maximum = 100000000;
            numCout.Minimum = 0;
            numCout.Increment = 10;

            // Configuration de la date
            dateAcquisition.Format = DateTimePickerFormat.Short;
            dateAcquisition.MaxDate = DateTime.Today;

            // Configuration des listes déroulantes
            cmbSpecialite.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSpecialite.DataSource =
                Enum.GetValues(typeof(Specialite));

            cmbStatut.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatut.DataSource =
                Enum.GetValues(typeof(StatutEquipement));

            cmbStatut.SelectedItem = StatutEquipement.EnService;

            // Ordre de navigation avec la touche Tab
            txtReference.TabIndex = 0;
            txtDesignation.TabIndex = 1;
            txtMarque.TabIndex = 2;
            txtLocalisation.TabIndex = 3;
            dateAcquisition.TabIndex = 4;
            numCout.TabIndex = 5;
            cmbSpecialite.TabIndex = 6;
            cmbStatut.TabIndex = 7;

            // Configuration de ErrorProvider
            errorProvider.ContainerControl = this;
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            errorProvider.SetIconPadding(txtReference, 5);
            errorProvider.SetIconPadding(txtDesignation, 5);
            errorProvider.SetIconPadding(txtMarque, 5);
            errorProvider.SetIconPadding(txtLocalisation, 5);

            // Validation en temps réel
            if (equipementOriginal == null)
            {
                txtReference.TextChanged +=
                    (s, e) => ValiderChamp(txtReference);
            }

            txtDesignation.TextChanged +=
                (s, e) => ValiderChamp(txtDesignation);

            txtMarque.TextChanged +=
                (s, e) => ValiderChamp(txtMarque);

            txtLocalisation.TextChanged +=
                (s, e) => ValiderChamp(txtLocalisation);

            numCout.ValueChanged +=
                (s, e) => ValiderCout();

            // Validation avant de quitter un champ
            if (equipementOriginal == null)
            {
                txtReference.Validating +=
                    (s, e) => e.Cancel = !ValiderChamp(txtReference);
            }

            txtDesignation.Validating +=
                (s, e) => e.Cancel = !ValiderChamp(txtDesignation);

            txtMarque.Validating +=
                (s, e) => e.Cancel = !ValiderChamp(txtMarque);

            txtLocalisation.Validating +=
                (s, e) => e.Cancel = !ValiderChamp(txtLocalisation);

            numCout.Validating +=
                (s, e) => e.Cancel = !ValiderCout();

            cmbSpecialite.Validating +=
                (s, e) => e.Cancel = !ValiderCombo(
                    cmbSpecialite,
                    "Veuillez sélectionner une spécialité."
                );

            cmbStatut.Validating +=
                (s, e) => e.Cancel = !ValiderCombo(
                    cmbStatut,
                    "Veuillez sélectionner un statut."
                );

            // Bouton Enregistrer
            Button btnValider = new Button
            {
                Text = "Enregistrer",
                Location = new Point(175, 405),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                CausesValidation = true
            };

            // Bouton Annuler
            Button btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(295, 405),
                Size = new Size(110, 40),
                DialogResult = DialogResult.Cancel,
                CausesValidation = false
            };

            btnValider.Click += BtnValider_Click;

            Controls.Add(btnValider);
            Controls.Add(btnAnnuler);

            AcceptButton = btnValider;
            CancelButton = btnAnnuler;
        }

        // Afficher un message d'erreur
        private void AfficherErreur(Control champ, string message)
        {
            errorProvider.SetError(champ, message);
            champ.BackColor = Color.MistyRose;
        }

        // Effacer le message d'erreur
        private void EffacerErreur(Control champ)
        {
            errorProvider.SetError(champ, "");
            champ.BackColor = Color.White;
        }

        // Vérifier les champs texte
        private bool ValiderChamp(Control champ)
        {
            string valeur = champ.Text.Trim();
            string message = "";

            if (champ == txtReference)
            {
                if (string.IsNullOrWhiteSpace(valeur))
                    message = "La référence est obligatoire.";

                else if (valeur.Length < 3 || valeur.Length > 20)
                    message = "La référence doit contenir entre 3 et 20 caractères.";

                else if (!Regex.IsMatch(valeur, @"^[A-Za-z0-9-]+$"))
                    message = "Utilisez uniquement des lettres, chiffres et tirets.";
            }
            else if (champ == txtDesignation)
            {
                if (string.IsNullOrWhiteSpace(valeur))
                    message = "La désignation est obligatoire.";

                else if (valeur.Length < 3)
                    message = "Minimum 3 caractères.";

                else if (!Regex.IsMatch(
                    valeur,
                    @"^[\p{L}\p{N}][\p{L}\p{N} .'-]*$"
                ))
                    message = "La désignation contient des caractères non autorisés.";
            }
            else if (champ == txtMarque)
            {
                if (string.IsNullOrWhiteSpace(valeur))
                    message = "La marque est obligatoire.";

                else if (valeur.Length < 2)
                    message = "La marque doit contenir au moins 2 caractères.";
            }
            else if (champ == txtLocalisation)
            {
                if (string.IsNullOrWhiteSpace(valeur))
                    message = "La localisation est obligatoire.";

                else if (valeur.Length < 2)
                    message = "La localisation doit contenir au moins 2 caractères.";
            }

            if (message != "")
            {
                AfficherErreur(champ, message);
                return false;
            }

            EffacerErreur(champ);
            return true;
        }

        // Vérifier le coût
        private bool ValiderCout()
        {
            if (numCout.Value <= 0)
            {
                AfficherErreur(
                    numCout,
                    "Le coût d'acquisition doit être supérieur à zéro."
                );

                return false;
            }

            EffacerErreur(numCout);
            return true;
        }

        // Vérifier les listes déroulantes
        private bool ValiderCombo(ComboBox combo, string message)
        {
            if (combo.SelectedItem == null)
            {
                AfficherErreur(combo, message);
                return false;
            }

            EffacerErreur(combo);
            return true;
        }

        // Vérifier toutes les données avant l'enregistrement
        private bool ValiderSaisie()
        {
            if (equipementOriginal == null &&
                !ValiderChamp(txtReference))
            {
                txtReference.Focus();
                return false;
            }

            if (!ValiderChamp(txtDesignation))
            {
                txtDesignation.Focus();
                return false;
            }

            if (!ValiderChamp(txtMarque))
            {
                txtMarque.Focus();
                return false;
            }

            if (!ValiderChamp(txtLocalisation))
            {
                txtLocalisation.Focus();
                return false;
            }

            if (!ValiderCout())
            {
                numCout.Focus();
                return false;
            }

            if (!ValiderCombo(
                cmbSpecialite,
                "Veuillez sélectionner une spécialité."
            ))
            {
                cmbSpecialite.Focus();
                return false;
            }

            if (!ValiderCombo(
                cmbStatut,
                "Veuillez sélectionner un statut."
            ))
            {
                cmbStatut.Focus();
                return false;
            }

            if (dateAcquisition.Value.Date > DateTime.Today)
            {
                MessageBox.Show(
                    "La date d'acquisition ne peut pas être dans le futur.",
                    "Date invalide",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                dateAcquisition.Focus();
                return false;
            }

            return true;
        }

        // Enregistrer l'équipement
        private void BtnValider_Click(object? sender, EventArgs e)
        {
            if (!ValiderSaisie())
                return;

            try
            {
                if (equipementOriginal == null)
                {
                    // Création d'un nouvel équipement
                    EquipementCree = new Equipement(
                        0,
                        txtReference.Text.Trim(),
                        txtDesignation.Text.Trim(),
                        txtMarque.Text.Trim(),
                        txtLocalisation.Text.Trim(),
                        dateAcquisition.Value.Date,
                        numCout.Value,
                        (Specialite)cmbSpecialite.SelectedItem!
                    );
                }
                else
                {
                    // Modification d'un équipement existant
                    EquipementCree = new Equipement(
                        equipementOriginal.Id,
                        equipementOriginal.Reference,
                        txtDesignation.Text.Trim(),
                        txtMarque.Text.Trim(),
                        txtLocalisation.Text.Trim(),
                        equipementOriginal.DateAcquisition,
                        equipementOriginal.CoutAcquisition,
                        equipementOriginal.SpecialiteRequise
                    );
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de l'enregistrement :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Libérer les ressources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                errorProvider.Dispose();

            base.Dispose(disposing);
        }
    }
}
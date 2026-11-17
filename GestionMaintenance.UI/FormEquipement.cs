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

        private readonly Button btnChoisirCarte = new();
        private readonly Button btnValider = new();
        private readonly Button btnAnnuler = new();

        private readonly ErrorProvider errorProvider = new();
        private readonly Equipement? equipementOriginal;

        public Equipement? EquipementCree { get; private set; }

        public StatutEquipement StatutSelectionne =>
            (StatutEquipement)cmbStatut.SelectedItem!;

        public FormEquipement()
        {
            InitialiserInterface();
        }

        public FormEquipement(Equipement equipement)
        {
            equipementOriginal = equipement;

            InitialiserInterface();

            txtReference.Text = equipement.Reference;
            txtDesignation.Text = equipement.Designation;
            txtMarque.Text = equipement.Marque;
            txtLocalisation.Text = equipement.Localisation;

            dateAcquisition.Value = equipement.DateAcquisition;
            numCout.Value = equipement.CoutAcquisition;

            cmbSpecialite.SelectedItem = equipement.SpecialiteRequise;
            cmbStatut.SelectedItem = equipement.Statut;

            // Champs non modifiables lors d'une modification.
            txtReference.ReadOnly = true;
            dateAcquisition.Enabled = false;
            numCout.Enabled = false;
            cmbSpecialite.Enabled = false;
        }

        private void InitialiserInterface()
        {
            Text = equipementOriginal == null
                ? "Ajouter un équipement"
                : "Modifier un équipement";

            Width = 500;
            Height = 650;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            errorProvider.ContainerControl = this;
            errorProvider.BlinkStyle =
                System.Windows.Forms.ErrorBlinkStyle.NeverBlink;

            AjouterLabel("Référence :", 25);
            ConfigurerTextBox(txtReference, 25);

            AjouterLabel("Désignation :", 75);
            ConfigurerTextBox(txtDesignation, 75);

            AjouterLabel("Marque :", 125);
            ConfigurerTextBox(txtMarque, 125);

            AjouterLabel("Localisation :", 175);
            ConfigurerTextBox(txtLocalisation, 175);

            AjouterLabel("Date d'acquisition :", 225);
            dateAcquisition.Location = new Point(175, 225);
            dateAcquisition.Width = 250;
            dateAcquisition.Format = DateTimePickerFormat.Short;
            dateAcquisition.MaxDate = DateTime.Today;
            Controls.Add(dateAcquisition);

            AjouterLabel("Coût d'acquisition :", 275);
            numCout.Location = new Point(175, 275);
            numCout.Width = 250;
            numCout.DecimalPlaces = 2;
            numCout.Minimum = 0;
            numCout.Maximum = 1000000000;
            Controls.Add(numCout);

            AjouterLabel("Spécialité :", 325);
            cmbSpecialite.Location = new Point(175, 325);
            cmbSpecialite.Width = 250;
            cmbSpecialite.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSpecialite.DataSource = Enum.GetValues(typeof(Specialite));
            Controls.Add(cmbSpecialite);

            AjouterLabel("Statut :", 375);
            cmbStatut.Location = new Point(175, 375);
            cmbStatut.Width = 250;
            cmbStatut.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatut.DataSource = Enum.GetValues(typeof(StatutEquipement));
            Controls.Add(cmbStatut);

            btnChoisirCarte.Text = "Choisir sur la carte";
            btnChoisirCarte.Location = new Point(175, 440);
            btnChoisirCarte.Size = new Size(250, 35);
            btnChoisirCarte.Click += BtnChoisirCarte_Click;
            Controls.Add(btnChoisirCarte);

            btnValider.Text = "Enregistrer";
            btnValider.Location = new Point(175, 510);
            btnValider.Size = new Size(115, 40);
            btnValider.Click += BtnValider_Click;
            Controls.Add(btnValider);

            btnAnnuler.Text = "Annuler";
            btnAnnuler.Location = new Point(310, 510);
            btnAnnuler.Size = new Size(115, 40);
            btnAnnuler.DialogResult = DialogResult.Cancel;
            Controls.Add(btnAnnuler);

            AcceptButton = btnValider;
            CancelButton = btnAnnuler;
        }

        private void AjouterLabel(string texte, int y)
        {
            Label label = new()
            {
                Text = texte,
                Location = new Point(25, y + 4),
                AutoSize = true
            };

            Controls.Add(label);
        }

        private void ConfigurerTextBox(TextBox textBox, int y)
        {
            textBox.Location = new Point(175, y);
            textBox.Width = 250;
            Controls.Add(textBox);
        }

        private void BtnChoisirCarte_Click(object? sender, EventArgs e)
        {
            using FormSelectionGPS formulaire = new();

            if (formulaire.ShowDialog(this) == DialogResult.OK)
            {
                txtLocalisation.Text =
                    formulaire.AdresseSelectionnee ?? "";

                errorProvider.SetError(txtLocalisation, "");
            }
        }

        private bool AfficherErreur(Control champ, string message)
        {
            errorProvider.SetError(champ, message);
            champ.Focus();

            MessageBox.Show(
                message,
                "Erreur de saisie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private bool ValiderFormulaire()
        {
            errorProvider.Clear();

            string reference = txtReference.Text.Trim();

            if (string.IsNullOrWhiteSpace(reference))
            {
                return AfficherErreur(
                    txtReference,
                    "La référence est obligatoire.");
            }

            if (!Regex.IsMatch(reference, @"^[a-zA-Z0-9-]{3,20}$"))
            {
                return AfficherErreur(
                    txtReference,
                    "La référence doit contenir entre 3 et 20 caractères : lettres, chiffres ou tirets.");
            }

            if (txtDesignation.Text.Trim().Length < 3)
            {
                return AfficherErreur(
                    txtDesignation,
                    "La désignation doit contenir au moins 3 caractères.");
            }

            if (txtMarque.Text.Trim().Length < 2)
            {
                return AfficherErreur(
                    txtMarque,
                    "La marque doit contenir au moins 2 caractères.");
            }

            if (txtLocalisation.Text.Trim().Length < 2)
            {
                return AfficherErreur(
                    txtLocalisation,
                    "La localisation doit contenir au moins 2 caractères.");
            }

            if (dateAcquisition.Value.Date > DateTime.Today)
            {
                return AfficherErreur(
                    dateAcquisition,
                    "La date d'acquisition ne peut pas être dans le futur.");
            }

            if (numCout.Value <= 0)
            {
                return AfficherErreur(
                    numCout,
                    "Le coût d'acquisition doit être supérieur à zéro.");
            }

            if (cmbSpecialite.SelectedItem == null)
            {
                return AfficherErreur(
                    cmbSpecialite,
                    "Veuillez choisir une spécialité.");
            }

            if (cmbStatut.SelectedItem == null)
            {
                return AfficherErreur(
                    cmbStatut,
                    "Veuillez choisir un statut.");
            }

            return true;
        }

        private void BtnValider_Click(object? sender, EventArgs e)
        {
            if (!ValiderFormulaire())
                return;

            int id = equipementOriginal?.Id ?? 0;

            EquipementCree = new Equipement(
                id,
                txtReference.Text.Trim(),
                txtDesignation.Text.Trim(),
                txtMarque.Text.Trim(),
                txtLocalisation.Text.Trim(),
                dateAcquisition.Value,
                numCout.Value,
                (Specialite)cmbSpecialite.SelectedItem!
            );

            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                errorProvider.Dispose();

            base.Dispose(disposing);
        }
    }
}
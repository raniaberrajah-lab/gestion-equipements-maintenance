using System;
using System.Drawing;
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

            numCout.Value = Math.Max(
                numCout.Minimum,
                Math.Min(numCout.Maximum, equipement.CoutAcquisition)
            );

            cmbSpecialite.SelectedItem = equipement.SpecialiteRequise;
            cmbStatut.SelectedItem = equipement.Statut;

            txtReference.ReadOnly = true;
            dateAcquisition.Enabled = false;
            numCout.Enabled = false;
            cmbSpecialite.Enabled = false;
        }

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

            AjouterChamp("Référence :", txtReference, 25);
            AjouterChamp("Désignation :", txtDesignation, 70);
            AjouterChamp("Marque :", txtMarque, 115);
            AjouterChamp("Localisation :", txtLocalisation, 160);
            AjouterChamp("Date acquisition :", dateAcquisition, 205);
            AjouterChamp("Coût acquisition :", numCout, 250);
            AjouterChamp("Spécialité :", cmbSpecialite, 295);
            AjouterChamp("Statut :", cmbStatut, 340);

            numCout.DecimalPlaces = 2;
            numCout.Maximum = 100000000;
            numCout.Minimum = 0;
            numCout.Increment = 10;

            dateAcquisition.Format = DateTimePickerFormat.Short;
            dateAcquisition.MaxDate = DateTime.Today;

            cmbSpecialite.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSpecialite.DataSource =
                Enum.GetValues(typeof(Specialite));

            cmbStatut.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatut.DataSource =
                Enum.GetValues(typeof(StatutEquipement));

            cmbStatut.SelectedItem = StatutEquipement.EnService;

            Button btnValider = new Button
            {
                Text = "Enregistrer",
                Location = new Point(175, 405),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Button btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(295, 405),
                Size = new Size(110, 40),
                DialogResult = DialogResult.Cancel
            };

            btnValider.Click += BtnValider_Click;

            Controls.Add(btnValider);
            Controls.Add(btnAnnuler);

            AcceptButton = btnValider;
            CancelButton = btnAnnuler;
        }

        private void BtnValider_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReference.Text) ||
                string.IsNullOrWhiteSpace(txtDesignation.Text))
            {
                MessageBox.Show(
                    "La référence et la désignation sont obligatoires."
                );
                return;
            }

            if (cmbSpecialite.SelectedItem == null ||
                cmbStatut.SelectedItem == null)
            {
                MessageBox.Show("Sélectionnez une spécialité et un statut.");
                return;
            }

            try
            {
                if (equipementOriginal == null)
                {
                    EquipementCree = new Equipement(
                        0,
                        txtReference.Text.Trim(),
                        txtDesignation.Text.Trim(),
                        txtMarque.Text.Trim(),
                        txtLocalisation.Text.Trim(),
                        dateAcquisition.Value.Date,
                        numCout.Value,
                        (Specialite)cmbSpecialite.SelectedItem
                    );
                }
                else
                {
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
                    "Erreur : " + ex.Message,
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}

#nullable enable

using System;

namespace GestionMaintenance;

/// <summary>
/// Technicien chargé de réaliser les interventions ; possède son propre planning.
/// </summary>
public class Technicien : Personne
{
    private decimal _tauxHoraire;

    public Specialite Specialite { get; set; }

    public PlanningTechnicien Planning { get; }

    /// <summary>
    /// Coût horaire (en dinars) utilisé pour calculer la main-d'œuvre.
    /// </summary>
    public decimal TauxHoraire
    {
        get => _tauxHoraire;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Le taux horaire ne peut pas être négatif.");

            _tauxHoraire = value;
        }
    }

    // Constructeur utilisé par Entity Framework
    private Technicien()
        : base(0, "", "", "", "")
    {
        Planning = new PlanningTechnicien();
    }

    public Technicien(
        int id,
        string nom,
        string prenom,
        string email,
        string telephone,
        Specialite specialite,
        decimal tauxHoraire)
        : base(id, nom, prenom, email, telephone)
    {
        Specialite = specialite;
        TauxHoraire = tauxHoraire;
        Planning = new PlanningTechnicien();
    }

    public override string ToString() =>
        $"{NomComplet} [{Specialite}] {TauxHoraire:N2} DT/h";
}
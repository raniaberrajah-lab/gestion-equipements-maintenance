#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionMaintenance;

/// <summary>
/// Équipement soumis à maintenance.
/// </summary>
public class Equipement : IEntite
{
    private readonly List<Intervention> _interventions = new();

    public int Id { get; private set; }

    public string Reference { get; private set; }

    public string Designation { get; set; }

    public string Marque { get; set; }

    public string Localisation { get; set; }

    public DateTime DateAcquisition { get; private set; }

    public decimal CoutAcquisition { get; private set; }

    public Specialite SpecialiteRequise { get; private set; }

    public StatutEquipement Statut { get; private set; }
        = StatutEquipement.EnService;

    public IReadOnlyList<Intervention> Interventions => _interventions;

    public Equipement(
        int id,
        string reference,
        string designation,
        string marque,
        string localisation,
        DateTime dateAcquisition,
        decimal coutAcquisition,
        Specialite specialiteRequise)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException(
                "Référence obligatoire.",
                nameof(reference));

        if (string.IsNullOrWhiteSpace(designation))
            throw new ArgumentException(
                "Désignation obligatoire.",
                nameof(designation));

        if (coutAcquisition < 0)
            throw new ArgumentOutOfRangeException(
                nameof(coutAcquisition),
                "Le coût d'acquisition ne peut pas être négatif.");

        Id = id;
        Reference = reference;
        Designation = designation;
        Marque = marque;
        Localisation = localisation;
        DateAcquisition = dateAcquisition;
        CoutAcquisition = coutAcquisition;
        SpecialiteRequise = specialiteRequise;
    }

    public void MettreEnPanne()
    {
        if (Statut == StatutEquipement.HorsService)
            throw new TransitionInvalideException(
                $"{Designation} est hors service.");

        Statut = StatutEquipement.EnPanne;
    }

    public void MettreEnMaintenance()
    {
        Statut = StatutEquipement.EnMaintenance;
    }

    public void RemettreEnService()
    {
        Statut = StatutEquipement.EnService;
    }

    public void MettreHorsService()
    {
        Statut = StatutEquipement.HorsService;
    }

    internal void AjouterIntervention(Intervention intervention)
    {
        if (intervention is null)
            throw new ArgumentNullException(nameof(intervention));

        if (!_interventions.Contains(intervention))
            _interventions.Add(intervention);
    }

    public decimal CoutTotalMaintenance()
    {
        return _interventions
            .Where(i => i.Statut == StatutIntervention.Terminee)
            .Sum(i => i.CoutReel);
    }

    public int NombrePannes()
    {
        return _interventions.Count(i =>
            i is InterventionCorrective &&
            i.Statut == StatutIntervention.Terminee);
    }

    public decimal TauxCoutMaintenance()
    {
        return CoutAcquisition == 0
            ? 0
            : CoutTotalMaintenance() / CoutAcquisition;
    }

    public override string ToString()
    {
        return $"{Reference} - {Designation} [{Statut}]";
    }
}
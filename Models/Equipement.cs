#nullable enable
using System.Collections.Generic;
using System.Linq;
using System;

namespace GestionMaintenance;

/// <summary>Équipement (machine, appareil, installation) soumis à maintenance.</summary>
public class Equipement : IEntite
{
    private readonly List<Intervention> _interventions = new();

    public int Id { get; }
    public string Reference { get; }
    public string Designation { get; set; }
    public string Marque { get; set; }
    public string Localisation { get; set; }
    public DateTime DateAcquisition { get; }
    public decimal CoutAcquisition { get; }
    public Specialite SpecialiteRequise { get; }
    public StatutEquipement Statut { get; private set; } = StatutEquipement.EnService;

    public IReadOnlyList<Intervention> Interventions => _interventions;

    public Equipement(int id, string reference, string designation, string marque, string localisation,
                      DateTime dateAcquisition, decimal coutAcquisition, Specialite specialiteRequise)
    {
        if (string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("Référence obligatoire.", nameof(reference));
        if (coutAcquisition < 0) throw new ArgumentOutOfRangeException(nameof(coutAcquisition));
        Id = id;
        Reference = reference;
        Designation = designation;
        Marque = marque;
        Localisation = localisation;
        DateAcquisition = dateAcquisition;
        CoutAcquisition = coutAcquisition;
        SpecialiteRequise = specialiteRequise;
    }

    // ---- Gestion du statut (les changements passent par des méthodes métier) ----
    public void MettreEnPanne()
    {
        if (Statut == StatutEquipement.HorsService)
            throw new TransitionInvalideException($"{Designation} est hors service.");
        Statut = StatutEquipement.EnPanne;
    }

    public void MettreEnMaintenance() => Statut = StatutEquipement.EnMaintenance;
    public void RemettreEnService() => Statut = StatutEquipement.EnService;
    public void MettreHorsService() => Statut = StatutEquipement.HorsService;

    internal void AjouterIntervention(Intervention intervention)
    {
        if (!_interventions.Contains(intervention))
            _interventions.Add(intervention);
    }

    // ---- Indicateurs de maintenance ----
    public decimal CoutTotalMaintenance() =>
        _interventions.Where(i => i.Statut == StatutIntervention.Terminee).Sum(i => i.CoutReel);

    public int NombrePannes() =>
        _interventions.Count(i => i is InterventionCorrective && i.Statut == StatutIntervention.Terminee);

    /// <summary>Part du coût d'acquisition déjà dépensée en maintenance (aide à décider d'un remplacement).</summary>
    public decimal TauxCoutMaintenance() =>
        CoutAcquisition == 0 ? 0 : CoutTotalMaintenance() / CoutAcquisition;

    public override string ToString() => $"{Reference} - {Designation} [{Statut}]";
}

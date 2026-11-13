#nullable enable
using System.Collections.Generic;
using System.Linq;
using System;

namespace GestionMaintenance;

/// <summary>
/// Classe abstraite d'une intervention de maintenance.
/// Le polymorphisme porte sur <see cref="TypeIntervention"/> et <see cref="CoutSupplementaire"/>.
/// </summary>
public abstract class Intervention : IEntite
{
    private readonly List<LignePiece> _pieces = new();

    public int Id { get; }
    public string Description { get; set; }
    public PrioriteIntervention Priorite { get; set; }
    public StatutIntervention Statut { get; private set; } = StatutIntervention.Planifiee;

    public Equipement Equipement { get; }
    public Technicien Technicien { get; }
    public ResponsableMaintenance PlanifieePar { get; }

    public DateTime DatePlanifiee { get; }
    public double DureePrevueHeures { get; }
    public DateTime? DateDebutReelle { get; private set; }
    public DateTime? DateFinReelle { get; private set; }

    public IReadOnlyList<LignePiece> Pieces => _pieces;

    protected Intervention(int id, Equipement equipement, Technicien technicien, ResponsableMaintenance planifieePar,
                           DateTime datePlanifiee, double dureePrevueHeures, PrioriteIntervention priorite, string description)
    {
        if (dureePrevueHeures <= 0)
            throw new ArgumentOutOfRangeException(nameof(dureePrevueHeures), "La durée prévue doit être positive.");
        Id = id;
        Equipement = equipement ?? throw new ArgumentNullException(nameof(equipement));
        Technicien = technicien ?? throw new ArgumentNullException(nameof(technicien));
        PlanifieePar = planifieePar ?? throw new ArgumentNullException(nameof(planifieePar));
        DatePlanifiee = datePlanifiee;
        DureePrevueHeures = dureePrevueHeures;
        Priorite = priorite;
        Description = description;
    }

    /// <summary>"Préventive" ou "Corrective" (défini par les classes filles).</summary>
    public abstract string TypeIntervention { get; }

    /// <summary>Coût propre au type d'intervention (ex. perte de production pour une panne). 0 par défaut.</summary>
    public virtual decimal CoutSupplementaire => 0m;

    public Creneau CreneauPrevu => new(DatePlanifiee, DatePlanifiee.AddHours(DureePrevueHeures));

    // ---- Durées et coûts ----
    public double DureeReelleHeures =>
        DateDebutReelle.HasValue && DateFinReelle.HasValue
            ? (DateFinReelle.Value - DateDebutReelle.Value).TotalHours
            : 0;

    public decimal CoutPieces => _pieces.Sum(l => l.Montant);

    public decimal CoutMainOeuvreReel => (decimal)DureeReelleHeures * Technicien.TauxHoraire;

    /// <summary>Coût réel = main-d'œuvre + pièces + supplément. N'existe qu'une fois l'intervention terminée.</summary>
    public decimal CoutReel =>
        Statut == StatutIntervention.Terminee ? CoutMainOeuvreReel + CoutPieces + CoutSupplementaire : 0m;

    /// <summary>Budget prévisionnel basé sur la durée prévue (utile pour la planification).</summary>
    public decimal CoutEstime => (decimal)DureePrevueHeures * Technicien.TauxHoraire + CoutPieces;

    // ---- Cycle de vie ----
    public void Demarrer(DateTime debut)
    {
        if (Statut != StatutIntervention.Planifiee)
            throw new TransitionInvalideException($"Impossible de démarrer l'intervention #{Id} (statut : {Statut}).");
        DateDebutReelle = debut;
        Statut = StatutIntervention.EnCours;
        Equipement.MettreEnMaintenance();
    }

    public void AjouterPiece(PieceDetachee piece, int quantite)
    {
        if (Statut != StatutIntervention.EnCours)
            throw new TransitionInvalideException("Les pièces ne peuvent être saisies que pendant l'intervention.");
        piece.Retirer(quantite);                       // lève StockInsuffisantException si besoin
        _pieces.Add(new LignePiece(piece, quantite));
    }

    public void Cloturer(DateTime fin)
    {
        if (Statut != StatutIntervention.EnCours || DateDebutReelle is null)
            throw new TransitionInvalideException($"Impossible de clôturer l'intervention #{Id} (statut : {Statut}).");
        if (fin <= DateDebutReelle.Value)
            throw new MaintenanceException("La date de fin doit être postérieure à la date de début.");
        DateFinReelle = fin;
        Statut = StatutIntervention.Terminee;
        Equipement.RemettreEnService();
    }

    public void Annuler()
    {
        if (Statut != StatutIntervention.Planifiee)
            throw new TransitionInvalideException($"Seule une intervention planifiée peut être annulée (statut : {Statut}).");
        Statut = StatutIntervention.Annulee;
    }

    public override string ToString() =>
        $"#{Id} {TypeIntervention} | {Equipement.Designation} | {Technicien.NomComplet} | {CreneauPrevu} | {Statut}";
}

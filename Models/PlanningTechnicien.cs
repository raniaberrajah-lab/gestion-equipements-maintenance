#nullable enable
using System.Collections.Generic;
using System.Linq;
using System;

namespace GestionMaintenance;

/// <summary>Agenda d'un technicien : détecte les conflits d'horaires et calcule la charge de travail.</summary>
public class PlanningTechnicien
{
    private readonly List<Intervention> _interventions = new();

    public IReadOnlyList<Intervention> Interventions => _interventions;

    private static bool EstActive(Intervention i) =>
        i.Statut == StatutIntervention.Planifiee || i.Statut == StatutIntervention.EnCours;

    /// <summary>Retourne l'intervention active qui chevauche le créneau, ou null si aucun conflit.</summary>
    public Intervention? ObtenirConflit(Creneau creneau) =>
        _interventions.FirstOrDefault(i => EstActive(i) && i.CreneauPrevu.Chevauche(creneau));

    public bool EstDisponible(Creneau creneau) => ObtenirConflit(creneau) is null;

    internal void Ajouter(Intervention intervention)
    {
        var conflit = ObtenirConflit(intervention.CreneauPrevu);
        if (conflit is not null)
            throw new ConflitPlanningException(
                $"{intervention.Technicien.NomComplet} est déjà affecté à l'intervention #{conflit.Id} ({conflit.CreneauPrevu}).");
        _interventions.Add(intervention);
    }

    /// <summary>Interventions non annulées dont la date planifiée est dans [du ; au[, triées par date.</summary>
    public IEnumerable<Intervention> Entre(DateTime du, DateTime au) =>
        _interventions
            .Where(i => i.Statut != StatutIntervention.Annulee && i.DatePlanifiee >= du && i.DatePlanifiee < au)
            .OrderBy(i => i.DatePlanifiee);

    public double ChargeHeures(DateTime du, DateTime au) => Entre(du, au).Sum(i => i.DureePrevueHeures);
}

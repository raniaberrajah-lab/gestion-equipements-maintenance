#nullable enable
using System;

namespace GestionMaintenance;

/// <summary>
/// Maintenance planifiée à périodicité fixe.
/// </summary>
public class InterventionPreventive : Intervention
{
    // Constructeur réservé à EF Core
    private InterventionPreventive() : base()
    {
    }

    public int PeriodiciteJours { get; private set; }

    // Constructeur métier conservé
    public InterventionPreventive(
        int id,
        Equipement equipement,
        Technicien technicien,
        ResponsableMaintenance planifieePar,
        DateTime datePlanifiee,
        double dureePrevueHeures,
        PrioriteIntervention priorite,
        string description,
        int periodiciteJours)
        : base(
            id,
            equipement,
            technicien,
            planifieePar,
            datePlanifiee,
            dureePrevueHeures,
            priorite,
            description)
    {
        if (periodiciteJours <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(periodiciteJours));

        PeriodiciteJours = periodiciteJours;
    }

    public override string TypeIntervention => "Préventive";

    /// <summary>
    /// Date de la prochaine visite préventive.
    /// </summary>
    public DateTime DateProchaine =>
        (DateFinReelle ?? DatePlanifiee)
        .AddDays(PeriodiciteJours);
}
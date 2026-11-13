#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Maintenance planifiée à périodicité fixe (graissage, contrôle, remplacement de filtre...).</summary>
public class InterventionPreventive : Intervention
{
    public int PeriodiciteJours { get; }

    public InterventionPreventive(int id, Equipement equipement, Technicien technicien, ResponsableMaintenance planifieePar,
                                  DateTime datePlanifiee, double dureePrevueHeures, PrioriteIntervention priorite,
                                  string description, int periodiciteJours)
        : base(id, equipement, technicien, planifieePar, datePlanifiee, dureePrevueHeures, priorite, description)
    {
        if (periodiciteJours <= 0) throw new ArgumentOutOfRangeException(nameof(periodiciteJours));
        PeriodiciteJours = periodiciteJours;
    }

    public override string TypeIntervention => "Préventive";

    /// <summary>Date de la prochaine visite préventive.</summary>
    public DateTime DateProchaine => (DateFinReelle ?? DatePlanifiee).AddDays(PeriodiciteJours);
}

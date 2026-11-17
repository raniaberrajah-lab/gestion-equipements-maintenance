#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Niveau d'urgence d'une intervention.</summary>
public enum PrioriteIntervention
{
    Basse = 1,
    Normale = 2,
    Haute = 3,
    Critique = 4
}

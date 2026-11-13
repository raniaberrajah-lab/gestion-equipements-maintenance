#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Cycle de vie d'une intervention : Planifiée → EnCours → Terminée (ou Annulée).</summary>
public enum StatutIntervention
{
    Planifiee,
    EnCours,
    Terminee,
    Annulee
}

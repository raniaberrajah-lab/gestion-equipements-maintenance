#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>État courant d'un équipement.</summary>
public enum StatutEquipement
{
    EnService,
    EnPanne,
    EnMaintenance,
    HorsService
}

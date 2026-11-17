#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Le technicien est déjà occupé sur le créneau demandé.</summary>
public class ConflitPlanningException : MaintenanceException
{
    public ConflitPlanningException(string message) : base(message) { }
}

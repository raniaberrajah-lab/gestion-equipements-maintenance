#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Changement de statut interdit (ex. clôturer une intervention non démarrée).</summary>
public class TransitionInvalideException : MaintenanceException
{
    public TransitionInvalideException(string message) : base(message) { }
}

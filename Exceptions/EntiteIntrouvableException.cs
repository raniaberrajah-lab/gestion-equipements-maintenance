#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Entité recherchée absente du dépôt.</summary>
public class EntiteIntrouvableException : MaintenanceException
{
    public EntiteIntrouvableException(string message) : base(message) { }
}

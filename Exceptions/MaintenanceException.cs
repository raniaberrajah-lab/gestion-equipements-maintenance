#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Exception métier de base de l'application.</summary>
public class MaintenanceException : Exception
{
    public MaintenanceException(string message) : base(message) { }
}

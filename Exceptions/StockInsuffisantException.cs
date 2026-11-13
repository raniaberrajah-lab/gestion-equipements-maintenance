#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Le stock d'une pièce est insuffisant pour la quantité demandée.</summary>
public class StockInsuffisantException : MaintenanceException
{
    public StockInsuffisantException(string message) : base(message) { }
}

#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Contrat de toute entité persistable (identifiant unique).</summary>
public interface IEntite
{
    int Id { get; }
}

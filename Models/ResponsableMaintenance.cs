#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Responsable qui planifie les interventions et suit les coûts.</summary>
public class ResponsableMaintenance : Personne
{
    public string Service { get; set; }

    public ResponsableMaintenance(int id, string nom, string prenom, string email, string telephone, string service)
        : base(id, nom, prenom, email, telephone)
    {
        Service = service;
    }
}

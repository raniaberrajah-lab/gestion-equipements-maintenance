#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Dépôt générique (pattern Repository) : isole la logique métier du stockage.</summary>
public interface IRepository<T> where T : class, IEntite
{
    int ProchainId();
    void Ajouter(T entite);
    T? Obtenir(int id);
    IEnumerable<T> ObtenirTous();
    bool Supprimer(int id);
}

#nullable enable
using System.Collections.Generic;
using System.Linq;
using System;

namespace GestionMaintenance;

/// <summary>Implémentation en mémoire (remplaçable par une base de données / fichier JSON).</summary>
public class InMemoryRepository<T> : IRepository<T> where T : class, IEntite
{
    private readonly Dictionary<int, T> _elements = new();
    private int _dernierId;

    public int ProchainId() => ++_dernierId;

    public void Ajouter(T entite)
    {
        ArgumentNullException.ThrowIfNull(entite);
        if (_elements.ContainsKey(entite.Id))
            throw new MaintenanceException($"Un élément d'identifiant {entite.Id} existe déjà.");
        _elements[entite.Id] = entite;
    }

    public T? Obtenir(int id) => _elements.TryGetValue(id, out var e) ? e : null;

    public IEnumerable<T> ObtenirTous() => _elements.Values.ToList();

    public bool Supprimer(int id) => _elements.Remove(id);
}

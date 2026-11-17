
#nullable enable

using System.Collections.Generic;
using System.Linq;
using System;

namespace GestionMaintenance;

/// <summary>
/// Implémentation du dépôt générique en mémoire.
/// </summary>
public class InMemoryRepository<T> : IRepository<T>
    where T : class, IEntite
{
    private readonly Dictionary<int, T> _elements = new();
    private int _dernierId;

    public int ProchainId()
    {
        if (_elements.Count > 0)
            _dernierId = Math.Max(_dernierId, _elements.Keys.Max());

        return ++_dernierId;
    }

    public void Ajouter(T entite)
    {
        ArgumentNullException.ThrowIfNull(entite);

        if (_elements.ContainsKey(entite.Id))
            throw new MaintenanceException(
                $"Un élément d'identifiant {entite.Id} existe déjà.");

        _elements[entite.Id] = entite;
    }

    public T? Obtenir(int id)
    {
        return _elements.TryGetValue(id, out var element)
            ? element
            : null;
    }

    public IEnumerable<T> ObtenirTous()
    {
        return _elements.Values.ToList();
    }

    // Modification d'un élément existant
    public void Modifier(T entite)
    {
        ArgumentNullException.ThrowIfNull(entite);

        if (!_elements.ContainsKey(entite.Id))
            throw new MaintenanceException(
                $"L'élément #{entite.Id} n'existe pas.");

        _elements[entite.Id] = entite;
    }

    // Suppression d'un élément
    public bool Supprimer(int id)
    {
        return _elements.Remove(id);
    }
}
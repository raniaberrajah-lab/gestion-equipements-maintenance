
#nullable enable
using System.Collections.Generic;

namespace GestionMaintenance;

/// <summary>
/// Dépôt générique : isole la logique métier du stockage.
/// </summary>
public interface IRepository<T> where T : class, IEntite
{
    int ProchainId();

    void Ajouter(T entite);

    T? Obtenir(int id);

    IEnumerable<T> ObtenirTous();

    void Modifier(T entite);

    bool Supprimer(int id);
}
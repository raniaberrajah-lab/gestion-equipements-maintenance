#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Classe abstraite : informations communes à toute personne du système.</summary>
public abstract class Personne : IEntite
{
    public int Id { get; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }

    protected Personne(int id, string nom, string prenom, string email, string telephone)
    {
        if (string.IsNullOrWhiteSpace(nom)) throw new ArgumentException("Le nom est obligatoire.", nameof(nom));
        if (string.IsNullOrWhiteSpace(prenom)) throw new ArgumentException("Le prénom est obligatoire.", nameof(prenom));
        Id = id;
        Nom = nom;
        Prenom = prenom;
        Email = email;
        Telephone = telephone;
    }

    public string NomComplet => $"{Prenom} {Nom}";

    public override string ToString() => $"{NomComplet} (#{Id})";
}

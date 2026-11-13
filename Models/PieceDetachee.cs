#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Pièce de rechange gérée en stock.</summary>
public class PieceDetachee : IEntite
{
    public int Id { get; }
    public string Reference { get; }
    public string Designation { get; set; }
    public decimal PrixUnitaire { get; set; }
    public int Stock { get; private set; }
    public int SeuilAlerte { get; set; }

    public PieceDetachee(int id, string reference, string designation, decimal prixUnitaire, int stock, int seuilAlerte)
    {
        if (string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("Référence obligatoire.", nameof(reference));
        if (prixUnitaire < 0) throw new ArgumentOutOfRangeException(nameof(prixUnitaire));
        if (stock < 0) throw new ArgumentOutOfRangeException(nameof(stock));
        Id = id;
        Reference = reference;
        Designation = designation;
        PrixUnitaire = prixUnitaire;
        Stock = stock;
        SeuilAlerte = seuilAlerte;
    }

    public bool EstEnAlerte => Stock <= SeuilAlerte;

    public void Approvisionner(int quantite)
    {
        if (quantite <= 0) throw new ArgumentOutOfRangeException(nameof(quantite), "La quantité doit être positive.");
        Stock += quantite;
    }

    public void Retirer(int quantite)
    {
        if (quantite <= 0) throw new ArgumentOutOfRangeException(nameof(quantite), "La quantité doit être positive.");
        if (quantite > Stock)
            throw new StockInsuffisantException(
                $"Stock insuffisant pour « {Designation} » : demandé {quantite}, disponible {Stock}.");
        Stock -= quantite;
    }

    public override string ToString() => $"{Reference} - {Designation} (stock : {Stock})";
}

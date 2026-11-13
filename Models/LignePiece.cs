#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Classe d'association Intervention ↔ PieceDetachee : quantité utilisée et prix figé à la date d'usage.</summary>
public class LignePiece
{
    public PieceDetachee Piece { get; }
    public int Quantite { get; }
    public decimal PrixUnitaireApplique { get; }

    public LignePiece(PieceDetachee piece, int quantite)
    {
        Piece = piece ?? throw new ArgumentNullException(nameof(piece));
        Quantite = quantite;
        PrixUnitaireApplique = piece.PrixUnitaire;   // le prix ultérieur de la pièce ne modifie pas l'historique
    }

    public decimal Montant => Quantite * PrixUnitaireApplique;
}

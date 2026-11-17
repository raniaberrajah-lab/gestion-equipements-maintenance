
#nullable enable
using System;

namespace GestionMaintenance;

public class LignePiece
{
    public int Id { get; private set; }

    public int PieceDetacheeId { get; private set; }

    public PieceDetachee Piece { get; private set; } = null!;

    public int Quantite { get; private set; }

    public decimal PrixUnitaireApplique { get; private set; }

    // Constructeur réservé à Entity Framework Core
    private LignePiece()
    {
    }

    // Constructeur utilisé dans l'application
    public LignePiece(PieceDetachee piece, int quantite)
    {
        Piece = piece ?? throw new ArgumentNullException(nameof(piece));

        if (quantite <= 0)
            throw new ArgumentException("La quantité doit être positive.");

        Quantite = quantite;
        PieceDetacheeId = piece.Id;
        PrixUnitaireApplique = piece.PrixUnitaire;
    }

    public decimal Montant => Quantite * PrixUnitaireApplique;
}

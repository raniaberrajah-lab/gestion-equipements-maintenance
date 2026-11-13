#nullable enable
using System.Collections.Generic;
using System;

namespace GestionMaintenance;

/// <summary>Objet-valeur immuable représentant une plage horaire [Début ; Fin[.</summary>
public readonly record struct Creneau
{
    public DateTime Debut { get; }
    public DateTime Fin { get; }

    public Creneau(DateTime debut, DateTime fin)
    {
        if (fin <= debut)
            throw new ArgumentException("La fin du créneau doit être postérieure à son début.");
        Debut = debut;
        Fin = fin;
    }

    public TimeSpan Duree => Fin - Debut;

    /// <summary>Deux créneaux se chevauchent si chacun commence avant la fin de l'autre.</summary>
    public bool Chevauche(Creneau autre) => Debut < autre.Fin && autre.Debut < Fin;

    public override string ToString() => $"{Debut:dd/MM/yyyy HH:mm} → {Fin:HH:mm}";
}

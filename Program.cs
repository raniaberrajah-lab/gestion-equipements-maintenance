
#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GestionMaintenance;

public static class Program
{
    private static void Titre(string texte)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 78));
        Console.WriteLine(texte);
        Console.WriteLine(new string('=', 78));
    }

    private static void Essayer(string libelle, Action action)
    {
        try
        {
            action();
        }
        catch (MaintenanceException ex)
        {
            Console.WriteLine(
                $"[Règle métier refusée] {libelle} -> {ex.GetType().Name} : {ex.Message}");
        }
    }

    public static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture =
            CultureInfo.GetCultureInfo("fr-FR");

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var service = new ServiceMaintenance();

        var responsable = new ResponsableMaintenance(
            1,
            "Ben Salah",
            "Mourad",
            "m.bensalah@usine.tn",
            "71 000 000",
            "Maintenance"
        );

        // ---------- Données de base ----------
        Titre("1. Création des équipements, techniciens et pièces");

        var compresseur = service.AjouterEquipement(
            "EQ-001",
            "Compresseur d'air",
            "Atlas Copco",
            "Atelier A",
            new DateTime(2022, 3, 15),
            18000m,
            Specialite.Mecanique
        );

        var climatiseur = service.AjouterEquipement(
            "EQ-002",
            "Climatiseur salle serveurs",
            "Daikin",
            "Bâtiment B",
            new DateTime(2023, 6, 1),
            7500m,
            Specialite.Froid
        );

        var onduleur = service.AjouterEquipement(
            "EQ-003",
            "Onduleur 20 kVA",
            "Eaton",
            "Local technique",
            new DateTime(2021, 9, 10),
            12000m,
            Specialite.Electricite
        );

        var ahmed = service.AjouterTechnicien(
            "Trabelsi", "Ahmed", "a.trabelsi@usine.tn",
            "20 111 111", Specialite.Mecanique, 25m
        );

        var sana = service.AjouterTechnicien(
            "Gharbi", "Sana", "s.gharbi@usine.tn",
            "22 222 222", Specialite.Froid, 30m
        );

        var karim = service.AjouterTechnicien(
            "Mejri", "Karim", "k.mejri@usine.tn",
            "24 333 333", Specialite.Electricite, 28m
        );

        var courroie = service.AjouterPiece(
            "P-100", "Courroie de compresseur", 45m, 5, 2
        );

        var gaz = service.AjouterPiece(
            "P-200", "Gaz réfrigérant R410A (kg)", 80m, 3, 2
        );

        service.AjouterPiece(
            "P-300", "Fusible 63 A", 3m, 20, 5
        );

        foreach (var e in service.ListerEquipements())
            Console.WriteLine(e);

        foreach (var t in service.ListerTechniciens())
            Console.WriteLine(t);

        // ---------- Planification ----------
        Titre("2. Planification des interventions");

        var lundi = new DateTime(2026, 11, 16, 8, 0, 0);

        var preventive = service.PlanifierPreventive(
            compresseur.Id,
            ahmed.Id,
            responsable,
            lundi,
            2,
            PrioriteIntervention.Normale,
            "Révision trimestrielle",
            90
        );

        Console.WriteLine($"Planifiée : {preventive}");

        Essayer("Même technicien, créneau chevauchant", () =>
            service.PlanifierPreventive(
                compresseur.Id,
                ahmed.Id,
                responsable,
                lundi.AddHours(1),
                2,
                PrioriteIntervention.Basse,
                "Contrôle",
                30
            )
        );

        Essayer("Technicien sans la spécialité requise", () =>
            service.PlanifierPreventive(
                compresseur.Id,
                sana.Id,
                responsable,
                lundi.AddDays(1),
                2,
                PrioriteIntervention.Basse,
                "Contrôle",
                30
            )
        );

        service.DeclarerPanne(climatiseur.Id);

        var corrective = service.PlanifierCorrective(
            climatiseur.Id,
            sana.Id,
            responsable,
            lundi.AddDays(1).AddHours(2),
            3,
            PrioriteIntervention.Critique,
            "Température salle serveurs trop élevée",
            "Fuite de fluide frigorigène",
            coutHoraireArret: 40m
        );

        Console.WriteLine($"Planifiée : {corrective}");

        // ---------- Exécution ----------
        Titre("3. Exécution : démarrage, pièces utilisées, clôture");

        service.DemarrerIntervention(preventive.Id, lundi);

        service.UtiliserPiece(preventive.Id, courroie.Id, 1);

        service.CloturerIntervention(
            preventive.Id,
            lundi.AddHours(2.5)
        );

        Console.WriteLine(
            $"Préventive terminée : main-d'œuvre {preventive.CoutMainOeuvreReel:N2} DT, " +
            $"pièces {preventive.CoutPieces:N2} DT, total {preventive.CoutReel:N2} DT"
        );

        Console.WriteLine(
            $"Prochaine visite préventive : {preventive.DateProchaine:dd/MM/yyyy}"
        );

        var debutCorrective = lundi.AddDays(1).AddHours(2);

        service.DemarrerIntervention(corrective.Id, debutCorrective);

        service.UtiliserPiece(corrective.Id, gaz.Id, 2);

        Essayer("Quantité supérieure au stock", () =>
            service.UtiliserPiece(corrective.Id, gaz.Id, 5)
        );

        service.CloturerIntervention(
            corrective.Id,
            debutCorrective.AddHours(3.5)
        );

        Console.WriteLine(
            $"Corrective terminée : main-d'œuvre {corrective.CoutMainOeuvreReel:N2} DT, " +
            $"pièces {corrective.CoutPieces:N2} DT, arrêt {corrective.CoutSupplementaire:N2} DT, " +
            $"total {corrective.CoutReel:N2} DT"
        );

        Essayer("Clôturer deux fois", () =>
            service.CloturerIntervention(
                corrective.Id,
                debutCorrective.AddHours(5)
            )
        );

        // ---------- Intervention restée planifiée ----------
        var enRetard = service.PlanifierPreventive(
            onduleur.Id,
            karim.Id,
            responsable,
            lundi,
            1.5,
            PrioriteIntervention.Haute,
            "Test des batteries",
            180
        );

        // ---------- Rapports ----------
        Titre("4. Planning et suivi");

        Console.WriteLine(
            $"Planning de {ahmed.NomComplet} (semaine du 16/11) :"
        );

        foreach (var i in service.PlanningDuTechnicien(
            ahmed.Id,
            lundi.Date,
            lundi.Date.AddDays(7)))
        {
            Console.WriteLine("  " + i);
        }

        Console.WriteLine(
            $"Charge de {sana.NomComplet} : " +
            $"{sana.Planning.ChargeHeures(lundi.Date, lundi.Date.AddDays(7)):N1} h"
        );

        Console.WriteLine("Interventions en retard au 18/11/2026 :");

        foreach (var i in service.InterventionsEnRetard(
            new DateTime(2026, 11, 18)))
        {
            Console.WriteLine("  " + i);
        }

        Console.WriteLine(
            "Techniciens libres en Électricité le 16/11 de 8h à 10h : " +
            string.Join(
                ", ",
                service.TechniciensDisponibles(
                    new Creneau(lundi, lundi.AddHours(2)),
                    Specialite.Electricite
                ).Select(t => t.NomComplet)
            )
        );

        // ---------- Coûts ----------
        Titre("5. Coûts de maintenance");

        foreach (var (cle, cout) in service.CoutParEquipement())
        {
            Console.WriteLine($"  {cle,-40} {cout,10:N2} DT");
        }

        Console.WriteLine();

        foreach (var (cle, cout) in service.CoutParTechnicien())
        {
            Console.WriteLine($"  {cle,-40} {cout,10:N2} DT");
        }

        Console.WriteLine();

        foreach (var (cle, cout) in service.CoutParType())
        {
            Console.WriteLine(
                $"  Interventions {cle,-26} {cout,10:N2} DT"
            );
        }

        Console.WriteLine(
            $"\n  Coût total du 16/11 au 30/11 : " +
            $"{service.CoutTotal(lundi.Date, new DateTime(2026, 11, 30)):N2} DT"
        );

        Console.WriteLine(
            $"  Taux de coût de maintenance du climatiseur : " +
            $"{climatiseur.TauxCoutMaintenance():P2} du prix d'achat"
        );

        Console.WriteLine(
            "\nÉquipement le plus coûteux : " +
            service.EquipementsLesPlusCouteux(1).First()
        );

        Console.WriteLine(
            "Pièces à réapprovisionner : " +
            string.Join(
                ", ",
                service.PiecesEnAlerte().Select(p => p.ToString())
            )
        );

        // Évite l'avertissement "variable non utilisée"
        _ = enRetard;
    }
}
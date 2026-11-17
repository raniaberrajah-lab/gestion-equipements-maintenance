#nullable enable
using System.Collections.Generic;
using System.Linq;
using System;

namespace GestionMaintenance;

/// <summary>
/// Façade métier : orchestre équipements, techniciens, pièces et interventions.
/// Reçoit ses dépôts par le constructeur (injection de dépendances).
/// </summary>
public class ServiceMaintenance
{
    private readonly IRepository<Equipement> _equipements;
    private readonly IRepository<Technicien> _techniciens;
    private readonly IRepository<PieceDetachee> _pieces;
    private readonly IRepository<Intervention> _interventions;

    public ServiceMaintenance()
        : this(new InMemoryRepository<Equipement>(), new InMemoryRepository<Technicien>(),
               new InMemoryRepository<PieceDetachee>(), new InMemoryRepository<Intervention>())
    { }

    public ServiceMaintenance(IRepository<Equipement> equipements, IRepository<Technicien> techniciens,
                              IRepository<PieceDetachee> pieces, IRepository<Intervention> interventions)
    {
        _equipements = equipements;
        _techniciens = techniciens;
        _pieces = pieces;
        _interventions = interventions;
    }

    private static T Charger<T>(IRepository<T> depot, int id, string libelle) where T : class, IEntite =>
        depot.Obtenir(id) ?? throw new EntiteIntrouvableException($"{libelle} #{id} introuvable.");

    // ------------------------------------------------------------------ Équipements
    public Equipement AjouterEquipement(string reference, string designation, string marque, string localisation,
                                        DateTime dateAcquisition, decimal coutAcquisition, Specialite specialiteRequise)
    {
        if (_equipements.ObtenirTous().Any(e => e.Reference == reference))
            throw new MaintenanceException($"La référence « {reference} » existe déjà.");
        var equipement = new Equipement(_equipements.ProchainId(), reference, designation, marque, localisation,
                                        dateAcquisition, coutAcquisition, specialiteRequise);
        _equipements.Ajouter(equipement);
        return equipement;
    }

    public IEnumerable<Equipement> ListerEquipements() => _equipements.ObtenirTous();

    public void DeclarerPanne(int equipementId) => Charger(_equipements, equipementId, "Équipement").MettreEnPanne();

    // ------------------------------------------------------------------ Techniciens
    public Technicien AjouterTechnicien(string nom, string prenom, string email, string telephone,
                                        Specialite specialite, decimal tauxHoraire)
    {
        var technicien = new Technicien(_techniciens.ProchainId(), nom, prenom, email, telephone, specialite, tauxHoraire);
        _techniciens.Ajouter(technicien);
        return technicien;
    }

    public IEnumerable<Technicien> ListerTechniciens() => _techniciens.ObtenirTous();

    public IEnumerable<Technicien> TechniciensDisponibles(Creneau creneau, Specialite? specialite = null) =>
        _techniciens.ObtenirTous()
                    .Where(t => (specialite is null || t.Specialite == specialite) && t.Planning.EstDisponible(creneau));

    // ------------------------------------------------------------------ Pièces
    public PieceDetachee AjouterPiece(string reference, string designation, decimal prixUnitaire, int stock, int seuilAlerte)
    {
        var piece = new PieceDetachee(_pieces.ProchainId(), reference, designation, prixUnitaire, stock, seuilAlerte);
        _pieces.Ajouter(piece);
        return piece;
    }

    public void ApprovisionnerPiece(int pieceId, int quantite) =>
        Charger(_pieces, pieceId, "Pièce").Approvisionner(quantite);

    public IEnumerable<PieceDetachee> PiecesEnAlerte() => _pieces.ObtenirTous().Where(p => p.EstEnAlerte);

    // ------------------------------------------------------------------ Planification
    private (Equipement eq, Technicien tech) ValiderPlanification(int equipementId, int technicienId,
                                                                  DateTime date, double dureeHeures)
    {
        var eq = Charger(_equipements, equipementId, "Équipement");
        var tech = Charger(_techniciens, technicienId, "Technicien");

        if (eq.Statut == StatutEquipement.HorsService)
            throw new MaintenanceException($"{eq.Designation} est hors service : aucune intervention possible.");
        if (tech.Specialite != eq.SpecialiteRequise)
            throw new MaintenanceException(
                $"{tech.NomComplet} ({tech.Specialite}) n'a pas la spécialité requise ({eq.SpecialiteRequise}).");

        var creneau = new Creneau(date, date.AddHours(dureeHeures));   // lève ArgumentException si durée <= 0
        var conflit = tech.Planning.ObtenirConflit(creneau);
        if (conflit is not null)
            throw new ConflitPlanningException(
                $"{tech.NomComplet} est déjà occupé sur ce créneau (intervention #{conflit.Id}, {conflit.CreneauPrevu}).");

        return (eq, tech);
    }

    private void Enregistrer(Intervention intervention)
    {
        intervention.Technicien.Planning.Ajouter(intervention);
        intervention.Equipement.AjouterIntervention(intervention);
        _interventions.Ajouter(intervention);
    }

    public InterventionPreventive PlanifierPreventive(int equipementId, int technicienId, ResponsableMaintenance responsable,
                                                      DateTime date, double dureeHeures, PrioriteIntervention priorite,
                                                      string description, int periodiciteJours)
    {
        var (eq, tech) = ValiderPlanification(equipementId, technicienId, date, dureeHeures);
        var intervention = new InterventionPreventive(_interventions.ProchainId(), eq, tech, responsable,
                                                      date, dureeHeures, priorite, description, periodiciteJours);
        Enregistrer(intervention);
        return intervention;
    }

    public InterventionCorrective PlanifierCorrective(int equipementId, int technicienId, ResponsableMaintenance responsable,
                                                      DateTime date, double dureeHeures, PrioriteIntervention priorite,
                                                      string description, string causeDePanne, decimal coutHoraireArret = 0m)
    {
        var (eq, tech) = ValiderPlanification(equipementId, technicienId, date, dureeHeures);
        if (eq.Statut != StatutEquipement.EnPanne)
            throw new MaintenanceException("Déclarez d'abord la panne de l'équipement avant de planifier une réparation.");
        var intervention = new InterventionCorrective(_interventions.ProchainId(), eq, tech, responsable,
                                                      date, dureeHeures, priorite, description, causeDePanne, coutHoraireArret);
        Enregistrer(intervention);
        return intervention;
    }

    // ------------------------------------------------------------------ Exécution
    public void DemarrerIntervention(int interventionId, DateTime debut) =>
        Charger(_interventions, interventionId, "Intervention").Demarrer(debut);

    public void UtiliserPiece(int interventionId, int pieceId, int quantite) =>
        Charger(_interventions, interventionId, "Intervention")
            .AjouterPiece(Charger(_pieces, pieceId, "Pièce"), quantite);

    public void CloturerIntervention(int interventionId, DateTime fin) =>
        Charger(_interventions, interventionId, "Intervention").Cloturer(fin);

    public void AnnulerIntervention(int interventionId) =>
        Charger(_interventions, interventionId, "Intervention").Annuler();

    // ------------------------------------------------------------------ Suivi, planning, rapports
    public IEnumerable<Intervention> PlanningDuTechnicien(int technicienId, DateTime du, DateTime au) =>
        Charger(_techniciens, technicienId, "Technicien").Planning.Entre(du, au);

    public IEnumerable<Intervention> InterventionsEnRetard(DateTime maintenant) =>
        _interventions.ObtenirTous()
                      .Where(i => i.Statut == StatutIntervention.Planifiee && i.DatePlanifiee < maintenant)
                      .OrderBy(i => i.DatePlanifiee);

    public IEnumerable<Intervention> HistoriqueEquipement(int equipementId) =>
        Charger(_equipements, equipementId, "Équipement").Interventions.OrderBy(i => i.DatePlanifiee);

    public decimal CoutTotal(DateTime du, DateTime au) =>
        _interventions.ObtenirTous()
                      .Where(i => i.Statut == StatutIntervention.Terminee
                                  && i.DateFinReelle >= du && i.DateFinReelle < au)
                      .Sum(i => i.CoutReel);

    public Dictionary<string, decimal> CoutParEquipement() =>
        _equipements.ObtenirTous().ToDictionary(e => $"{e.Reference} - {e.Designation}", e => e.CoutTotalMaintenance());

    public Dictionary<string, decimal> CoutParTechnicien() =>
        _techniciens.ObtenirTous().ToDictionary(
            t => $"{t.NomComplet} (#{t.Id})",
            t => t.Planning.Interventions.Where(i => i.Statut == StatutIntervention.Terminee).Sum(i => i.CoutReel));

    public Dictionary<string, decimal> CoutParType() =>
        _interventions.ObtenirTous()
                      .Where(i => i.Statut == StatutIntervention.Terminee)
                      .GroupBy(i => i.TypeIntervention)
                      .ToDictionary(g => g.Key, g => g.Sum(i => i.CoutReel));

    public IEnumerable<Equipement> EquipementsLesPlusCouteux(int n) =>
        _equipements.ObtenirTous().OrderByDescending(e => e.CoutTotalMaintenance()).Take(n);
}

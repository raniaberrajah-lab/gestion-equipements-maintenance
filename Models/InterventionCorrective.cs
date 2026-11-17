#nullable enable
using System;

namespace GestionMaintenance;

/// <summary>
/// Réparation consécutive à une panne.
/// Peut générer un coût d'arrêt de production.
/// </summary>
public class InterventionCorrective : Intervention
{
    // Constructeur réservé à EF Core
    private InterventionCorrective() : base()
    {
        CauseDePanne = string.Empty;
    }

    public string CauseDePanne { get; set; }

    public decimal CoutHoraireArret { get; private set; }

    public double? HeuresArretSaisies { get; set; }

    // Constructeur métier conservé
    public InterventionCorrective(
        int id,
        Equipement equipement,
        Technicien technicien,
        ResponsableMaintenance planifieePar,
        DateTime datePlanifiee,
        double dureePrevueHeures,
        PrioriteIntervention priorite,
        string description,
        string causeDePanne,
        decimal coutHoraireArret)
        : base(
            id,
            equipement,
            technicien,
            planifieePar,
            datePlanifiee,
            dureePrevueHeures,
            priorite,
            description)
    {
        if (coutHoraireArret < 0)
            throw new ArgumentOutOfRangeException(
                nameof(coutHoraireArret));

        CauseDePanne = causeDePanne;
        CoutHoraireArret = coutHoraireArret;
    }

    public override string TypeIntervention => "Corrective";

    /// <summary>
    /// Durée d'arrêt : saisie manuelle,
    /// sinon durée réelle de la réparation.
    /// </summary>
    public double HeuresArret =>
        HeuresArretSaisies ?? DureeReelleHeures;

    public override decimal CoutSupplementaire =>
        (decimal)HeuresArret * CoutHoraireArret;
}
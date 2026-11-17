using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AjouterInterventionsEtPieces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Priorite = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DatePlanifiee = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DureePrevueHeures = table.Column<double>(type: "REAL", nullable: false),
                    DateDebutReelle = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateFinReelle = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    CauseDePanne = table.Column<string>(type: "TEXT", nullable: true),
                    CoutHoraireArret = table.Column<decimal>(type: "TEXT", nullable: true),
                    HeuresArretSaisies = table.Column<double>(type: "REAL", nullable: true),
                    PeriodiciteJours = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PiecesDetachees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Reference = table.Column<string>(type: "TEXT", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    SeuilAlerte = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PiecesDetachees", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "PiecesDetachees");
        }
    }
}

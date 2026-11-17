using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AjouterCoordonneesGPS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Equipements",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Equipements",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Equipements");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Equipements");
        }
    }
}

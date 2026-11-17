using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GestionMaintenance.Data
{
    public class AppDbContextFactory
        : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string dossierProjet = Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..", "..", "..", ".."
                )
            );

            string cheminBD = Path.Combine(
                dossierProjet,
                "gestion-maintenance.db"
            );

            var optionsBuilder =
                new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlite($"Data Source={cheminBD}");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
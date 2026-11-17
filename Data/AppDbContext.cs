using Microsoft.EntityFrameworkCore;

namespace GestionMaintenance.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // =========================================
        // TABLES DE LA BASE DE DONNEES
        // =========================================

        public DbSet<Equipement> Equipements { get; set; } = null!;
        public DbSet<Technicien> Techniciens { get; set; } = null!;
        public DbSet<PieceDetachee> PiecesDetachees { get; set; } = null!;
        public DbSet<Intervention> Interventions { get; set; } = null!;

        // =========================================
        // CONFIGURATION DES ENTITES
        // =========================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================
            // PERSONNES ET TECHNICIENS
            // =========================================

            modelBuilder.Entity<Personne>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Personnes");
                entity.Ignore(p => p.NomComplet);
            });

            modelBuilder.Entity<Technicien>(entity =>
            {
                entity.ToTable("Personnes");
                entity.Ignore(t => t.Planning);
            });

            // =========================================
            // EQUIPEMENTS
            // =========================================

            modelBuilder.Entity<Equipement>(entity =>
            {
                entity.ToTable("Equipements");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Reference)
                      .IsUnique();

                entity.Property(e => e.Reference)
                      .IsRequired();

                entity.Property(e => e.Designation)
                      .IsRequired();

                // Navigation non enregistrée directement
                entity.Ignore(e => e.Interventions);
            });

            // =========================================
            // PIECES DETACHEES
            // =========================================

            modelBuilder.Entity<PieceDetachee>(entity =>
            {
                entity.ToTable("PiecesDetachees");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Reference)
                      .IsRequired();

                entity.Property(p => p.Designation)
                      .IsRequired();
            });

            // =========================================
            // INTERVENTIONS
            // =========================================

            modelBuilder.Entity<Intervention>(entity =>
            {
                entity.ToTable("Interventions");

                entity.HasKey(i => i.Id);

                // Héritage TPH
                entity.HasDiscriminator<string>("Discriminator")
                      .HasValue<InterventionPreventive>("PREVENTIVE")
                      .HasValue<InterventionCorrective>("CORRECTIVE");

                // Propriétés calculées ou non persistées
                entity.Ignore(i => i.TypeIntervention);
                entity.Ignore(i => i.CreneauPrevu);
                entity.Ignore(i => i.DureeReelleHeures);
                entity.Ignore(i => i.CoutPieces);
                entity.Ignore(i => i.CoutMainOeuvreReel);
                entity.Ignore(i => i.CoutReel);
                entity.Ignore(i => i.CoutEstime);
                entity.Ignore(i => i.CoutSupplementaire);
                entity.Ignore(i => i.Pieces);

                // Navigations temporairement ignorées
                entity.Ignore(i => i.Equipement);
                entity.Ignore(i => i.Technicien);
                entity.Ignore(i => i.PlanifieePar);
            });

            // =========================================
            // PROPRIETES DES CLASSES FILLES
            // =========================================

            modelBuilder.Entity<InterventionCorrective>(entity =>
            {
                entity.Ignore(i => i.HeuresArret);
            });

            modelBuilder.Entity<InterventionPreventive>(entity =>
            {
                entity.Ignore(i => i.DateProchaine);
            });
        }
    }
}
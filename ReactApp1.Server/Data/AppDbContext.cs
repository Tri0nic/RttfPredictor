using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Entities;

namespace ReactApp1.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PlayerStatsEntity> PlayerStats { get; set; }
        public DbSet<TournamentEntity> Tournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TournamentEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("tournaments");

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.StartsAt).HasColumnName("starts_at").HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<PlayerStatsEntity>(entity =>
            {
                // Составной первичный ключ: игрок + турнир
                entity.HasKey(e => new { e.PlayerId, e.TournamentId });

                entity.ToTable("player_stats");

                entity.Property(e => e.PlayerId).HasColumnName("player_id");
                entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
                entity.Property(e => e.Position).HasColumnName("position");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.City).HasColumnName("city");
                entity.Property(e => e.Year).HasColumnName("year");
                entity.Property(e => e.Arm).HasColumnName("arm");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.TournamentsPlayed).HasColumnName("tournaments_played");
                entity.Property(e => e.WonGames).HasColumnName("won_games");
                entity.Property(e => e.LostGames).HasColumnName("lost_games");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.Tournament)
                      .WithMany()
                      .HasForeignKey(e => e.TournamentId)
                      .HasPrincipalKey(t => t.Id);
            });
        }
    }
}

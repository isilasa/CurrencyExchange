using CurrencyExchange.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Currency> Currencies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Currency>(entity =>
            {
                entity.ToTable("currency");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rate)
                    .IsRequired()
                    .HasColumnType("decimal(18,6)");
            });

            builder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.Id);
            });

            builder.Entity<Favorite>(entity =>
            {
                entity.ToTable("favorites");

                entity.HasKey(x => new { x.UserId, x.CurrencyId });

                entity.HasOne(x => x.User)
                      .WithMany(u => u.Favorites)
                      .HasForeignKey(x => x.UserId);

                entity.HasOne(x => x.Currency)
                      .WithMany(c => c.Favorites)
                      .HasForeignKey(x => x.CurrencyId);
            });
        }
    }
}

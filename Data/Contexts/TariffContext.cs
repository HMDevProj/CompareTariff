using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Data.Contexts
{
    public class TariffContext : DbContext
    {
        public TariffContext(DbContextOptions<TariffContext> options) : base(options) { }

        public DbSet<Tariff> Tariffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tariff>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.HasIndex(t => t.Name).IsUnique();
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.BaseCost).IsRequired();
                entity.Property(t => t.AdditionalKwhCost).IsRequired();

                entity.HasData(
                    new Tariff { Id = 1, Name = "Product A", Type = 1, BaseCost = 100m, IncludedKwh = 1000, AdditionalKwhCost = 0.3m },
                    new Tariff { Id = 2, Name = "Product B", Type = 2, IncludedKwh = 2000, BaseCost = 200m, AdditionalKwhCost = 0.25m },
                    new Tariff { Id = 3, Name = "Test Product", Type = 1, BaseCost = 1m, IncludedKwh = 0, AdditionalKwhCost = 1m }
                );
            });
        }
    }
}

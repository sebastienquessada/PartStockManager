using Microsoft.EntityFrameworkCore;
using PartStockManager.Adapter.Database.Entities;

namespace PartStockManager.Adapter.Database.Context
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<PartEntity> Parts => Set<PartEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartEntity>(entity =>
            {
                entity.HasIndex(p => p.Reference)
                      .IsUnique();

                entity.Property(p => p.Reference)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.ToTable(t => t.HasCheckConstraint("CK_Part_Reference_MinLength", "LEN(Reference) >= 1"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Part_Name_MinLength", "LEN(Name) >= 1"));

                entity.ToTable(t => t.HasCheckConstraint("CK_Part_StockQuantity_GTE_Zero", "StockQuantity >= 0"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Part_LowStockThreshold_GTE_Zero", "LowStockThreshold >= 0"));
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext : DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options)
            : base(options)
    {

    }

    [Obsolete]
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(eb =>
        {
            eb.ToTable("Products");
            eb.Property(p => p.Sku)
                .HasMaxLength(20)
                .IsRequired();
            eb.Property(p => p.Name)
                .HasMaxLength(60)
                .IsRequired();
            eb.Property(p => p.CurrentUnitPrice)
                .HasPrecision(15, 2);
            eb.Property(p => p.InternalCode)
                .HasMaxLength(10);
            eb.Property(p => p.Description)
                .HasMaxLength(200);
            eb.Property(p => p.StockQuantity)
                .HasDefaultValue(0);


            // Restricción para que CurrentUnitPrice sea mayor a 0
            eb.HasCheckConstraint("CK_Product_CurrentUnitPrice_Positive", "[CurrentUnitPrice] > 0");
            // Restricción para que StockQuantity sea no negativo
            eb.HasCheckConstraint("CK_Product_Stock_NonNegative", "[StockQuantity] >= 0");
            modelBuilder.Entity<OrderItem>(eb =>
            {
                eb.ToTable("OrderItems");

                eb.Property(p => p.Quantity)
                    .HasPrecision(18, 2); // o el valor que quieras

                eb.Property(p => p.UnitPrice)
                    .HasPrecision(18, 2); // evita truncamientos
            });


        });
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CopilotSample.Api.Domain.Entities;
using CopilotSample.Api.Domain.Values;

namespace CopilotSample.Api.Infrastructure.Data;

// TODO: DbContext configuration with:
// - Product and Category DbSets
// - OnModelCreating to configure:
//   - Product: Name required, max 100 chars
//   - Product: SKU unique, max 50 chars
//   - Product: Price precision
//   - Category: Name required, max 50 chars
//   - Relationships between Product and Category
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(p => p.Description)
                .HasMaxLength(500);
                
            // Configure SKU value object conversion
            entity.Property(p => p.SKU)
                .HasConversion(
                    v => v.Value,           // Convert to string for storage
                    v => ProductSKU.Create(v), // Convert back to ProductSKU
                    new ValueComparer<ProductSKU>(
                        (l, r) => l.Value == r.Value,
                        v => v.Value.GetHashCode(),
                        v => ProductSKU.Create(v.Value)))
                .IsRequired()
                .HasMaxLength(50);
                
            entity.HasIndex(p => p.SKU)
                .IsUnique();
                
            // Configure Price value object conversion
            entity.Property(p => p.Price)
                .HasConversion(
                    v => v.Amount,          // Convert to decimal for storage
                    v => Money.FromDecimal(v, "JPY"), // Convert back to Money
                    new ValueComparer<Money>(
                        (l, r) => l.Amount == r.Amount && l.Currency == r.Currency,
                        v => HashCode.Combine(v.Amount, v.Currency),
                        v => Money.FromDecimal(v.Amount, v.Currency)))
                .HasPrecision(18, 2);
                
            // Configure Stock value object conversion
            entity.Property(p => p.Stock)
                .HasConversion(
                    v => v.Value,           // Convert to int for storage
                    v => StockQuantity.Create(v), // Convert back to StockQuantity
                    new ValueComparer<StockQuantity>(
                        (l, r) => l.Value == r.Value,
                        v => v.Value.GetHashCode(),
                        v => StockQuantity.Create(v.Value)));
                
            // Legacy StockQuantity property for backward compatibility (not stored)
            entity.Ignore(p => p.StockQuantity);
                
            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(c => c.Description)
                .HasMaxLength(200);
        });
        
        // Configure relationship
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Seed data for development
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Id = 2, Name = "Books", Description = "Physical and digital books" },
            new Category { Id = 3, Name = "Clothing", Description = "Apparel and accessories" }
        );
    }
}
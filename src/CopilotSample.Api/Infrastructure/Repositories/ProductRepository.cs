using Microsoft.EntityFrameworkCore;
using CopilotSample.Api.Domain.Entities;
using CopilotSample.Api.Domain.Values;
using CopilotSample.Api.Application.Interfaces;
using CopilotSample.Api.Infrastructure.Data;

namespace CopilotSample.Api.Infrastructure.Repositories;

// Repository focused on data access only - business logic moved to domain
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }
    
    public async Task<Product> AddAsync(Product product)
    {
        // Timestamps are managed by domain model
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        return product;
    }
    
    public async Task UpdateAsync(Product product)
    {
        // UpdatedAt is managed by domain model business methods
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> ExistsAsync(string sku)
    {
        return await _context.Products.AnyAsync(p => p.SKU == sku);
    }
    
    public async Task<bool> ExistsAsync(ProductSKU sku)
    {
        return await _context.Products.AnyAsync(p => p.SKU == sku);
    }
    
    // Search products by name or SKU (case-insensitive)
    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<Product>();
        }
        
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.ToLower().Contains(lowerSearchTerm) || 
                       p.SKU.Value.ToLower().Contains(lowerSearchTerm))
            .ToListAsync();
    }
}
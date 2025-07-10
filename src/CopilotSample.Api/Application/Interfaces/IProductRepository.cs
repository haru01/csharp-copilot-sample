using CopilotSample.Api.Domain.Entities;
using CopilotSample.Api.Domain.Values;

namespace CopilotSample.Api.Application.Interfaces;

// TODO: Repository interface for Product with:
// - GetAllAsync()
// - GetByIdAsync(id)
// - GetBySkuAsync(sku)
// - AddAsync(product)
// - UpdateAsync(product)
// - DeleteAsync(id)
// - ExistsAsync(sku)
public interface IProductRepository
{
    // Basic CRUD operations
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetBySkuAsync(string sku);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string sku);
    Task<bool> ExistsAsync(ProductSKU sku);
    
    // Search and filter operations
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
}
using CopilotSample.Api.Application.DTOs;
using CopilotSample.Api.Domain.Entities;

namespace CopilotSample.Api.Application.Services;

// TODO: Service interface for business logic
// - Search products by name or SKU
// - Calculate total inventory value
// - Check if product is in stock
// - Update stock after purchase
// - Get low stock products (less than threshold)
// - CRUD operations
public interface IProductService
{
    // CRUD operations
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(CreateProductDto dto);
    Task<bool> UpdateProductAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int id);
    
    // Business logic operations
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<decimal> CalculateTotalInventoryValueAsync();
    Task<bool> IsInStockAsync(int productId, int quantity);
    Task<bool> UpdateStockAsync(int productId, int quantityToDeduct);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5);
}
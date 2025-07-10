using CopilotSample.Api.Application.Interfaces;
using CopilotSample.Api.Application.DTOs;
using CopilotSample.Api.Domain.Entities;
using CopilotSample.Api.Domain.Values;

namespace CopilotSample.Api.Application.Services;

// Application service orchestrating domain operations
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    
    // CRUD operations using domain factory and methods
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _repository.GetAllAsync();
    }
    
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
    
    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        // Create value object from DTO for type safety
        var sku = ProductSKU.Create(dto.SKU);
        
        // Check SKU uniqueness at application layer (defense in depth)
        if (await _repository.ExistsAsync(sku))
        {
            throw new InvalidOperationException($"Product with SKU '{dto.SKU}' already exists");
        }
        
        // Use domain factory method with business rules
        var product = Product.Create(
            dto.Name, 
            dto.Description, 
            dto.Price, 
            dto.SKU, 
            dto.CategoryId, 
            dto.StockQuantity);
        
        return await _repository.AddAsync(product);
    }
    
    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var existingProduct = await _repository.GetByIdAsync(id);
        
        if (existingProduct == null)
        {
            return false;
        }
        
        // 部分更新：設定されたプロパティのみ更新
        if (dto.Name != null || dto.Description != null || dto.Price.HasValue)
        {
            existingProduct.UpdateBasicInfo(
                dto.Name ?? existingProduct.Name,
                dto.Description ?? existingProduct.Description,
                dto.Price ?? existingProduct.Price.Amount);
        }
        
        // SKUは更新不可（ビジネスルール）
        // 必要に応じて例外を発生させることも可能
        
        if (dto.CategoryId.HasValue)
        {
            existingProduct.ChangeCategory(dto.CategoryId.Value);
        }
        
        if (dto.StockQuantity.HasValue)
        {
            existingProduct.SetStockQuantity(dto.StockQuantity.Value);
        }
        
        await _repository.UpdateAsync(existingProduct);
        return true;
    }
    
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }
        
        // Use domain business rule for deletion
        if (!product.CanBeDeleted())
        {
            throw new InvalidOperationException("Cannot delete product with existing stock. Please reduce stock to zero first.");
        }
        
        await _repository.DeleteAsync(id);
        return true;
    }
    
    // Search operation (delegated to repository)
    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _repository.SearchAsync(searchTerm);
    }
    
    // Aggregate operations using domain methods
    public async Task<decimal> CalculateTotalInventoryValueAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Sum(p => (decimal)p.CalculateInventoryValue());
    }
    
    public async Task<bool> IsInStockAsync(int productId, int quantity)
    {
        var product = await _repository.GetByIdAsync(productId);
        if (product == null)
        {
            return false;
        }
        
        // Use domain method for stock checking
        return product.IsInStock(quantity);
    }
    
    public async Task<bool> UpdateStockAsync(int productId, int quantityToDeduct)
    {
        var product = await _repository.GetByIdAsync(productId);
        if (product == null)
        {
            return false;
        }
        
        try
        {
            // Use domain method for stock deduction with business rules
            product.DeductStock(quantityToDeduct);
            await _repository.UpdateAsync(product);
            return true;
        }
        catch (InvalidOperationException)
        {
            // Insufficient stock
            return false;
        }
        catch (ArgumentException)
        {
            // Invalid quantity
            return false;
        }
    }
    
    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5)
    {
        var products = await _repository.GetAllAsync();
        // Use domain method for low stock filtering
        return products.Where(p => p.IsLowStock(threshold)).OrderBy(p => p.StockQuantity);
    }
}
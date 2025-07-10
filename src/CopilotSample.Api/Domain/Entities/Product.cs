using CopilotSample.Api.Domain.Values;

namespace CopilotSample.Api.Domain.Entities;

// Rich domain model with business logic and value objects
public class Product
{
    // Private constructor to enforce factory method usage
    private Product()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Factory method to create new product with validation using value objects
    public static Product Create(string name, string? description, Money price, ProductSKU sku, int categoryId, StockQuantity initialStock)
    {
        var product = new Product();
        
        product.ValidateAndSetName(name);
        product.ValidateAndSetDescription(description);
        product.Price = price;
        product.SKU = sku;
        product.ValidateAndSetCategoryId(categoryId);
        product.Stock = initialStock;
        
        return product;
    }
    
    // Overload for decimal price (convenience method)
    public static Product Create(string name, string? description, decimal price, string sku, int categoryId, int initialStock = 0)
    {
        return Create(
            name, 
            description, 
            Money.FromDecimal(price), 
            ProductSKU.Create(sku), 
            categoryId, 
            Values.StockQuantity.Create(initialStock)
        );
    }
    
    // Properties with private setters for encapsulation
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money Price { get; private set; }
    public ProductSKU SKU { get; private set; }
    public StockQuantity Stock { get; private set; }
    public int CategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    // Navigation property (EF Core manages this)
    public Category Category { get; set; } = null!;
    
    // Legacy property for backward compatibility (maps to Stock.Value)
    public int StockQuantity 
    { 
        get => Stock.Value; 
        private set => Stock = Values.StockQuantity.Create(value); 
    }
    
    // Business methods for updating basic information
    public void UpdateBasicInfo(string name, string? description, Money price)
    {
        ValidateAndSetName(name);
        ValidateAndSetDescription(description);
        Price = price;
        UpdateTimestamp();
    }
    
    // Overload for decimal price
    public void UpdateBasicInfo(string name, string? description, decimal price)
    {
        UpdateBasicInfo(name, description, Money.FromDecimal(price));
    }
    
    public void UpdatePrice(Money price)
    {
        Price = price;
        UpdateTimestamp();
    }
    
    public void UpdatePrice(decimal price)
    {
        UpdatePrice(Money.FromDecimal(price));
    }
    
    public void ChangeSKU(ProductSKU sku)
    {
        SKU = sku;
        UpdateTimestamp();
    }
    
    public void ChangeSKU(string sku)
    {
        ChangeSKU(ProductSKU.Create(sku));
    }
    
    public void ChangeCategory(int categoryId)
    {
        ValidateAndSetCategoryId(categoryId);
        UpdateTimestamp();
    }
    
    // Stock management business methods using value objects
    public void AddStock(int quantity)
    {
        Stock = Stock.Add(quantity);
        UpdateTimestamp();
    }
    
    public void DeductStock(int quantity)
    {
        Stock = Stock.Deduct(quantity);
        UpdateTimestamp();
    }
    
    public void SetStockQuantity(int quantity)
    {
        Stock = Values.StockQuantity.Create(quantity);
        UpdateTimestamp();
    }
    
    public void SetStock(StockQuantity stock)
    {
        Stock = stock;
        UpdateTimestamp();
    }
    
    public bool IsInStock(int quantity)
    {
        return Stock.IsSufficient(quantity);
    }
    
    public bool IsLowStock(int threshold = 5)
    {
        return Stock.IsLowStock(threshold);
    }
    
    public bool IsOutOfStock()
    {
        return Stock.IsOutOfStock;
    }
    
    public bool CanBeDeleted()
    {
        return Stock.IsOutOfStock;
    }
    
    public Money CalculateInventoryValue()
    {
        return Price * Stock.Value;
    }
    
    // Get restock amount needed to reach target level
    public int GetRestockAmount(int targetLevel)
    {
        return Stock.GetRestockAmount(targetLevel);
    }
    
    // Check if price is in specific range
    public bool IsPriceInRange(Money minPrice, Money maxPrice)
    {
        return Price >= minPrice && Price <= maxPrice;
    }
    
    // Check if SKU has specific prefix
    public bool HasSKUPrefix(string prefix)
    {
        return SKU.HasPrefix(prefix);
    }
    
    // Private validation methods
    private void ValidateAndSetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Product name cannot exceed 100 characters", nameof(name));
        
        Name = name.Trim();
    }
    
    private void ValidateAndSetDescription(string? description)
    {
        if (description != null && description.Length > 500)
            throw new ArgumentException("Product description cannot exceed 500 characters", nameof(description));
        
        Description = description?.Trim();
    }
    
    private void ValidateAndSetCategoryId(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentException("Category ID must be greater than 0", nameof(categoryId));
        
        CategoryId = categoryId;
    }
    
    private void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
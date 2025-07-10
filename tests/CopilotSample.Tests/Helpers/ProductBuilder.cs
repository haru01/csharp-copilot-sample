using CopilotSample.Api.Domain.Entities;

namespace CopilotSample.Tests.Helpers;

// Builder pattern for creating Product test data with sensible defaults
// Allows overriding specific properties for test assertions while keeping others consistent
// Default values are chosen to be realistic and avoid conflicts in most test scenarios
public class ProductBuilder
{
    private static int _idCounter = 1000; // Start high to avoid conflicts with seeded data
    
    private string _name = "Test Product";
    private decimal _price = 50m; // 日本円では小数点なし
    private string _sku = "TEST-001"; // Will be made unique in constructor
    private int _stockQuantity = 20; // Sufficient stock for most tests
    private int _categoryId = 1;
    private Category? _category = null;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    public static ProductBuilder Create() 
    {
        var builder = new ProductBuilder
        {
            _sku = $"TEST-{_idCounter++:D3}" // Generate unique SKU
        };
        return builder;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithSku(string sku)
    {
        _sku = sku;
        return this;
    }

    public ProductBuilder WithStockQuantity(int stockQuantity)
    {
        _stockQuantity = stockQuantity;
        return this;
    }

    public ProductBuilder WithCategoryId(int categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    public ProductBuilder WithCategory(Category category)
    {
        _category = category;
        _categoryId = category.Id == 0 ? 1 : category.Id; // Use 1 if category.Id is not set
        return this;
    }

    public ProductBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ProductBuilder WithUpdatedAt(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    public Product Build()
    {
        // Ensure we have a valid category ID - if none provided, use default 1
        var categoryId = _categoryId > 0 ? _categoryId : 1;
        
        // Use domain factory method to create product
        var product = Product.Create(_name, null, _price, _sku, categoryId, _stockQuantity);
        
        // Set navigation property if provided
        if (_category != null)
        {
            product.Category = _category;
        }
        
        return product;
    }

    // Implicit conversion for convenience: ProductBuilder builder = ProductBuilder.Create(); Product product = builder;
    public static implicit operator Product(ProductBuilder builder) => builder.Build();
}
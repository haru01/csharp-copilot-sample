using CopilotSample.Api.Infrastructure.Data;
using CopilotSample.Api.Infrastructure.Repositories;
using CopilotSample.Api.Application.Services;
using CopilotSample.Api.Domain.Entities;
using CopilotSample.Tests.Helpers;

namespace CopilotSample.Tests.Services;

// Test: Should find products by name (case-insensitive)
// Test: Should find products by SKU (case-insensitive)
// Test: Should return empty when search term is empty
// Test: Should calculate correct total inventory value
// Test: Should return true when enough stock available
// Test: Should return false when not enough stock
// Test: Should update stock and return true when successful
// Test: Should return false when trying to deduct more than available
// Test: Should get products with low stock
public class ProductServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;
    private readonly ProductService _service;
    
    public ProductServiceTests()
    {
        _context = TestDbContextFactory.CreateDbContext();
        _repository = new ProductRepository(_context);
        _service = new ProductService(_repository);
    }
    
    [Fact]
    public async Task 商品名で大文字小文字を区別せずに検索できる()
    {
        // Arrange
        var targetName = "Gaming Laptop"; // This is what we're searching for
        var category = CategoryBuilder.Create();
        var matchingProduct = ProductBuilder.Create()
            .WithName(targetName) // Key attribute for search
            .WithCategory(category);
        var nonMatchingProduct = ProductBuilder.Create()
            .WithName("Different Product") // Different name - should not match
            .WithCategory(category);
        
        _context.Categories.Add(category);
        _context.Products.AddRange(matchingProduct, nonMatchingProduct);
        await _context.SaveChangesAsync();
        
        // Act - search with lowercase version of target name
        var results = await _service.SearchProductsAsync("gaming laptop");
        
        // Assert
        var products = results.ToList();
        Assert.Single(products);
        Assert.Equal(targetName, products[0].Name);
    }
    
    [Fact]
    public async Task SKUで大文字小文字を区別せずに検索できる()
    {
        // Arrange
        var targetSKU = "BOOK-12345"; // This is what we're searching for
        var category = CategoryBuilder.Create();
        var matchingProduct = ProductBuilder.Create()
            .WithSku(targetSKU) // Key attribute for search
            .WithCategory(category);
        var nonMatchingProduct = ProductBuilder.Create()
            .WithCategory(category); // Uses default SKU - should not match
        
        _context.Categories.Add(category);
        _context.Products.AddRange(matchingProduct, nonMatchingProduct);
        await _context.SaveChangesAsync();
        
        // Act - search with lowercase version of target SKU
        var results = await _service.SearchProductsAsync("book-12345");
        
        // Assert
        var products = results.ToList();
        Assert.Single(products);
        Assert.Equal(targetSKU, products[0].SKU);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task 無効な検索条件の場合は空の結果を返す(string searchTerm)
    {
        // Arrange
        // No products added - empty database
        
        // Act
        var results = await _service.SearchProductsAsync(searchTerm);
        
        // Assert
        Assert.Empty(results);
    }
    
    [Fact]
    public async Task 在庫総価値を正しく計算できる()
    {
        // Arrange
        var category = CategoryBuilder.Create();
        
        var expensiveProduct = ProductBuilder.Create()
            .WithPrice(1500m) // High price for calculation
            .WithStockQuantity(5) // 1500 * 5 = 7500
            .WithCategory(category);
        
        var cheapProduct = ProductBuilder.Create()
            .WithPrice(10m) // Low price for calculation
            .WithStockQuantity(20) // 10 * 20 = 200
            .WithCategory(category);
        
        _context.Categories.Add(category);
        _context.Products.AddRange(expensiveProduct, cheapProduct);
        await _context.SaveChangesAsync();
        
        // Act
        var total = await _service.CalculateTotalInventoryValueAsync();
        
        // Assert - Total should be sum of (price * stock) for all products
        Assert.Equal(7700m, total); // 7500 + 200 = 7700
    }
    
    [Fact]
    public async Task 十分な在庫がある場合にtrueを返す()
    {
        // Arrange
        var availableStock = 15; // This is the key value for the test
        var requestedQuantity = 8; // Less than available stock
        
        var category = CategoryBuilder.Create();
        var product = ProductBuilder.Create()
            .WithStockQuantity(availableStock) // Key attribute for stock check
            .WithCategory(category);
        
        _context.Categories.Add(category);
        var savedProduct = _context.Products.Add(product).Entity;
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.IsInStockAsync(savedProduct.Id, requestedQuantity);
        
        // Assert - Should return true because requested (8) < available (15)
        Assert.True(result);
    }
    
    [Fact]
    public async Task 在庫が不足している場合にfalseを返す()
    {
        // Arrange
        var availableStock = 8; // This is the key constraint
        var requestedQuantity = 12; // More than available stock
        
        var category = CategoryBuilder.Create();
        var product = ProductBuilder.Create()
            .WithStockQuantity(availableStock) // Key constraint for test
            .WithCategory(category);
        
        _context.Categories.Add(category);
        var savedProduct = _context.Products.Add(product).Entity;
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.IsInStockAsync(savedProduct.Id, requestedQuantity);
        
        // Assert - Should return false because requested (12) > available (8)
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(0)] // ゼロ数量 - 無効
    [InlineData(-1)] // 負の数量 - 無効
    public async Task 無効な数量の場合にfalseを返す(int invalidQuantity)
    {
        // Arrange
        var sufficientStock = 100; // High stock to ensure quantity is the only factor
        
        var category = CategoryBuilder.Create();
        var product = ProductBuilder.Create()
            .WithStockQuantity(sufficientStock) // High stock - quantity is the focus
            .WithCategory(category);
        
        _context.Categories.Add(category);
        var savedProduct = _context.Products.Add(product).Entity;
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.IsInStockAsync(savedProduct.Id, invalidQuantity);
        
        // Assert - Should return false for invalid quantities regardless of stock
        Assert.False(result);
    }
    
    [Fact]
    public async Task 在庫を正常に減らしてtrueを返す()
    {
        // Arrange
        var initialStock = 20; // Starting stock level
        var deductionAmount = 7; // Amount to deduct
        var expectedFinalStock = 13; // 20 - 7 = 13
        
        var category = CategoryBuilder.Create().WithName("Electronics");
        var product = ProductBuilder.Create()
            .WithStockQuantity(initialStock) // Key value for deduction test
            .WithName("Deductible Product")
            .WithSku("DEDUCT-001")
            .WithCategory(category);
        
        _context.Categories.Add(category);
        var savedProduct = _context.Products.Add(product).Entity;
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.UpdateStockAsync(savedProduct.Id, deductionAmount);
        
        // Assert
        Assert.True(result);
        
        // Verify stock was properly deducted
        var updatedProduct = await _context.Products.FindAsync(savedProduct.Id);
        Assert.Equal(expectedFinalStock, updatedProduct?.StockQuantity);
    }
    
    [Fact]
    public async Task 在庫不足で減らせない場合にfalseを返す()
    {
        // Arrange
        var insufficientStock = 5; // Low stock level
        var excessiveDeduction = 12; // More than available stock
        
        var category = CategoryBuilder.Create();
        var product = ProductBuilder.Create()
            .WithStockQuantity(insufficientStock) // Key constraint - insufficient stock
            .WithCategory(category);
        
        _context.Categories.Add(category);
        var savedProduct = _context.Products.Add(product).Entity;
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.UpdateStockAsync(savedProduct.Id, excessiveDeduction);
        
        // Assert
        Assert.False(result);
        
        // Verify stock was not changed when deduction fails
        var unchangedProduct = await _context.Products.FindAsync(savedProduct.Id);
        Assert.Equal(insufficientStock, unchangedProduct?.StockQuantity);
    }
    
    [Fact]
    public async Task 閾値を下回る在庫の商品を取得できる()
    {
        // Arrange
        var threshold = 10; // This is the key threshold for the test
        
        var category = CategoryBuilder.Create();
        
        var lowStockProduct1 = ProductBuilder.Create()
            .WithStockQuantity(3) // Below threshold (3 < 10)
            .WithCategory(category);
        
        var lowStockProduct2 = ProductBuilder.Create()
            .WithStockQuantity(7) // Below threshold (7 < 10)
            .WithCategory(category);
        
        var highStockProduct = ProductBuilder.Create()
            .WithStockQuantity(25) // Above threshold (25 >= 10)
            .WithCategory(category);
        
        _context.Categories.Add(category);
        _context.Products.AddRange(lowStockProduct1, lowStockProduct2, highStockProduct);
        await _context.SaveChangesAsync();
        
        // Act
        var results = await _service.GetLowStockProductsAsync(threshold);
        
        // Assert - Should return only products with stock < threshold
        var products = results.ToList();
        Assert.Equal(2, products.Count); // Two products below threshold
        Assert.All(products, p => Assert.True(p.StockQuantity < threshold));
        Assert.Contains(products, p => p.StockQuantity == 3); // First low stock product
        Assert.Contains(products, p => p.StockQuantity == 7); // Second low stock product
    }
    
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
using CopilotSample.Api.Infrastructure.Data;
using CopilotSample.Api.Infrastructure.Repositories;
using CopilotSample.Api.Domain.Entities;
using CopilotSample.Tests.Helpers;

namespace CopilotSample.Tests.Repositories;

// Test: Should return all products with categories
// Test: Should return product by ID with category
// Test: Should return null when product ID not found
// Test: Should create new product with timestamps
// Test: Should return true when SKU exists
// Test: Should return false when SKU does not exist
// Test: Should update product and change UpdatedAt
// Test: Should delete product by ID
public class ProductRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _context = TestDbContextFactory.CreateDbContext();
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Products_With_Categories()
    {
        // Arrange
        var category = CategoryBuilder.Create().WithName("Electronics");
        var product1 = ProductBuilder.Create().WithName("Laptop").WithCategory(category);
        var product2 = ProductBuilder.Create().WithName("Mouse").WithCategory(category);

        _context.Categories.Add(category);
        _context.Products.AddRange(product1, product2);
        await _context.SaveChangesAsync();

        // Act
        var products = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(products);
        var productList = products.ToList();
        Assert.Equal(2, productList.Count);
        Assert.All(productList, p => Assert.NotNull(p.Category));
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Product_With_Category()
    {
        // Arrange
        var categoryName = "Electronics";
        var productName = "Laptop";

        var category = CategoryBuilder.Create().WithName(categoryName);
        var product = ProductBuilder.Create().WithName(productName).WithCategory(category);

        _context.Categories.Add(category);
        var savedProduct = _context.Products.Add(product).Entity;
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(savedProduct.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productName, result.Name);
        Assert.NotNull(result.Category);
        Assert.Equal(categoryName, result.Category.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Act
        var product = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(product);
    }

    [Fact]
    public async Task AddAsync_Should_Create_Product_With_Timestamps()
    {
        // Arrange
        var newProduct = Product.Create("New Product", null, 29m, "NEW-001", 1, 5);

        // Act
        var createdProduct = await _repository.AddAsync(newProduct);

        // Assert
        Assert.NotEqual(0, createdProduct.Id);
        Assert.NotEqual(default(DateTime), createdProduct.CreatedAt);
        Assert.NotEqual(default(DateTime), createdProduct.UpdatedAt);
        // Use InRange to handle minor timestamp differences
        var timeDiff = Math.Abs((createdProduct.UpdatedAt - createdProduct.CreatedAt).TotalMilliseconds);
        Assert.InRange(timeDiff, 0, 100); // Allow up to 100ms difference
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_SKU_Exists()
    {
        // Arrange
        var targetSku = "LAPTOP-001";
        var category = CategoryBuilder.Create();
        var product = ProductBuilder.Create().WithSku(targetSku).WithCategory(category);

        _context.Categories.Add(category);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(targetSku);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_False_When_SKU_Not_Exists()
    {
        // Act
        var exists = await _repository.ExistsAsync("NOTEXIST-001");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Product_And_UpdatedAt()
    {
        // Arrange
        var category = CategoryBuilder.Create();
        var product = ProductBuilder.Create().WithName("Original Laptop").WithPrice(999m).WithCategory(category);

        _context.Categories.Add(category);
        var savedProduct = _context.Products.Add(product).Entity;
        await _context.SaveChangesAsync();

        var originalUpdatedAt = savedProduct.UpdatedAt;
        savedProduct.UpdateBasicInfo("Updated Laptop", savedProduct.Description, 1299m);

        // Add small delay to ensure UpdatedAt changes
        await Task.Delay(10);

        // Act
        await _repository.UpdateAsync(savedProduct);

        // Assert
        var updatedProduct = await _repository.GetByIdAsync(savedProduct.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Laptop", updatedProduct.Name);
        Assert.Equal(1299m, updatedProduct.Price);
        Assert.True(updatedProduct.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Product()
    {
        // Arrange
        var category = CategoryBuilder.Create();
        var product1 = ProductBuilder.Create().WithName("Product 1").WithCategory(category);
        var product2 = ProductBuilder.Create().WithName("Product 2").WithCategory(category);

        _context.Categories.Add(category);
        var savedProduct1 = _context.Products.Add(product1).Entity;
        _context.Products.Add(product2);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(savedProduct1.Id);

        // Assert
        var deletedProduct = await _repository.GetByIdAsync(savedProduct1.Id);
        Assert.Null(deletedProduct);

        var allProducts = await _repository.GetAllAsync();
        Assert.Single(allProducts);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
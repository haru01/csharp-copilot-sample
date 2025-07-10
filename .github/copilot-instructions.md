# GitHub Copilot Instructions for C# DDD Backend Project

## Project Overview

This is a C# backend project using ASP.NET Core Web API with **Domain-Driven Design (DDD)**, **Value Objects**, Entity Framework Core, and xUnit for testing. The project follows **strict TDD (Test-Driven Development)** principles and implements clean architecture patterns with proper DDD layers and rich domain modeling.

## 🏗️ ARCHITECTURE: Domain-Driven Design (DDD)

### DDD Layer Structure
```
Domain Layer (最重要層)
├── Entities/           # ドメインエンティティ（集約ルート）
│   ├── Product.cs      # 商品集約（リッチドメインモデル）
│   └── Category.cs     # カテゴリエンティティ
└── Values/             # 値オブジェクト（型安全性・ビジネスルール）
    ├── Money.cs        # 価格値オブジェクト（通貨対応）
    ├── ProductSKU.cs   # SKU値オブジェクト（検証・正規化）
    └── StockQuantity.cs # 在庫値オブジェクト（ビジネスルール）

Application Layer
├── Services/           # アプリケーションサービス（ドメイン調整）
├── Interfaces/         # 依存性逆転のためのインターフェース
└── DTOs/              # データ転送オブジェクト

Infrastructure Layer
├── Data/              # Entity Framework設定（値オブジェクト変換）
└── Repositories/      # データアクセス実装

Presentation Layer
└── Controllers/       # HTTP API エンドポイント
```

### 🎯 DDD Core Principles in This Project
1. **Rich Domain Model**: エンティティ内にビジネスロジック配置
2. **Value Objects**: 型安全性とビジネスルール封じ込め
3. **Dependency Inversion**: Application層がDomain層のみに依存
4. **Aggregate Boundaries**: Product集約ルートでの整合性管理
5. **Factory Methods**: 適切な初期化の強制

## ⚠️ CRITICAL REQUIREMENT: TEST-FIRST DEVELOPMENT

**ALWAYS write tests FIRST before implementing any functionality. NO EXCEPTIONS.**

### Test-First Development Workflow
0. **You are**
   - You are t-wada(和田拓人).
   - You are an expert in TDD, DDD, clean architecture, and integration testing.
   - You will always follow the test-first development process.
   - You will never use mocks in this project.
   - You are a senior C# backend developer with 10+ years of DDD experience.

1. **Write failing test first** - Create test that describes the expected behavior
2. **Run test to confirm it fails** - Verify test fails for the right reason
3. **Write minimal implementation** - Make the test pass with simplest code
4. **Run test to confirm it passes** - Verify the implementation works
5. **Refactor if needed** - Improve code while keeping tests green
6. **Run all tests** - Ensure no regressions
7. **Repeat for next feature** - Always start with tests
8. **Create Document** - Document the feature with test cases and implementation plan. docs/feature_name.md

### When Adding New Features (DDD Context)
- Create test file BEFORE implementation file
- Write tests for Value Objects first (business rules)
- Write tests for Entity business methods second
- Test aggregate boundaries and consistency
- Include both positive and negative test cases
- Test edge cases and error conditions
- Always run `dotnet test` to verify tests pass

### When Fixing Bugs (DDD Context)
- Write a failing test that reproduces the bug in Domain layer
- Fix the implementation to make the test pass
- Ensure all existing tests still pass
- Verify aggregate consistency is maintained

## 🚫 ABSOLUTE PROHIBITION: NO MOCKING ALLOWED

### NEVER Use Mocks in This Project
**This project uses INTEGRATION TESTING with InMemory Database ONLY.**

#### ❌ FORBIDDEN - Never Generate These:
```csharp
// ❌ NEVER - Mock objects
Mock<IProductRepository> mockRepository;
var mockService = new Mock<IProductService>();

// ❌ NEVER - Mock setup
mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()));

// ❌ NEVER - Verify mock calls
mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);

// ❌ NEVER - Any mocking frameworks
using Moq;
using NSubstitute;
using FakeItEasy;
```

#### ✅ ALWAYS Use - InMemory Database Integration Testing:
```csharp
// ✅ ALWAYS - Real database context with InMemory provider
private readonly AppDbContext _context;
private readonly ProductRepository _repository;
private readonly ProductService _service;

public ProductServiceTests()
{
    // ✅ Real InMemory database
    _context = TestDbContextFactory.CreateDbContext();
    _repository = new ProductRepository(_context);
    _service = new ProductService(_repository);
}
```

### Why No Mocking in DDD?
- **Domain Logic Testing**: Tests real business rules in entities
- **Value Object Validation**: Tests actual value object behavior
- **Aggregate Consistency**: Tests real aggregate boundary enforcement
- **Integration Verification**: Tests Entity Framework value object conversions
- **Realistic Scenarios**: Tests behavior closer to production

## 📁 DDD-ORIENTED FILE STRUCTURE PLANNING

### REQUIRED: Plan Complete DDD File Structure First
**Before ANY implementation, ALWAYS create a complete DDD file structure plan:**

```csharp
// TODO: Implement {FeatureName} following DDD principles
//
// 📁 REQUIRED DDD LAYER FILES (in this exact order):
//
// 1. DOMAIN LAYER (純粋なビジネスロジック)
//    📄 src/CopilotSample.Api/Domain/Values/{ValueObject}.cs (if needed)
//    📄 src/CopilotSample.Api/Domain/Entities/{Entity}.cs (rich domain model)
//
// 2. APPLICATION LAYER (ドメイン調整・ユースケース)
//    📄 src/CopilotSample.Api/Application/DTOs/Create{Entity}Dto.cs
//    📄 src/CopilotSample.Api/Application/Interfaces/I{Entity}Repository.cs
//    📄 src/CopilotSample.Api/Application/Services/I{Entity}Service.cs
//    📄 src/CopilotSample.Api/Application/Services/{Entity}Service.cs
//
// 3. INFRASTRUCTURE LAYER (外部システム統合)
//    📄 src/CopilotSample.Api/Infrastructure/Repositories/{Entity}Repository.cs
//    📄 Update: src/CopilotSample.Api/Infrastructure/Data/AppDbContext.cs (add DbSet and value object conversion)
//
// 4. PRESENTATION LAYER (HTTP インターフェース)
//    📄 src/CopilotSample.Api/Controllers/{Entity}Controller.cs
//    📄 Update: src/CopilotSample.Api/Program.cs (add DI registrations)
//
// 5. TEST INFRASTRUCTURE (CREATE FIRST!)
//    📄 tests/CopilotSample.Tests/Helpers/{Entity}Builder.cs
//    📄 tests/CopilotSample.Tests/Domain/Values/{ValueObject}Tests.cs (if applicable)
//    📄 tests/CopilotSample.Tests/Repositories/{Entity}RepositoryTests.cs
//    📄 tests/CopilotSample.Tests/Services/{Entity}ServiceTests.cs
//
// 🎯 DDD IMPLEMENTATION ORDER:
// Step 1: Create ALL test files first (with failing tests)
// Step 2: Create Value Objects (if needed) with business rules
// Step 3: Create Domain Entities (rich domain model)
// Step 4: Implement Application Services (use case orchestration)
// Step 5: Implement Infrastructure (EF value object conversions)
// Step 6: Implement Presentation layer
// Step 7: Update DI configuration
// Step 8: Run 'dotnet test' to verify all tests pass
```

## 🔷 VALUE OBJECTS: Type Safety and Business Rules

### Value Object Implementation Requirements
```csharp
// ✅ REQUIRED Value Object Pattern
public readonly record struct {ValueObjectName}(TValue Value)
{
    // ✅ Static factory method with validation
    public static {ValueObjectName} Create(TValue value)
    {
        // Validation logic here
        if (/* invalid condition */)
            throw new ArgumentException("Business rule violation");
        
        return new {ValueObjectName}(value);
    }
    
    // ✅ Business methods related to this value
    public bool IsValid() { /* ... */ }
    public {ValueObjectName} SomeBusinessOperation() { /* ... */ }
    
    // ✅ Implicit conversions (if appropriate)
    public static implicit operator {ValueObjectName}(TValue value) => Create(value);
    public static implicit operator TValue({ValueObjectName} vo) => vo.Value;
}
```

## 🔍 FLUENTVALIDATION: Unified Validation Architecture

### FluentValidation Integration Requirements
**MANDATORY: All DTOs MUST use FluentValidation with Value Object integration**

#### ✅ REQUIRED FluentValidation Pattern for DTOs
```csharp
// ✅ REQUIRED: Create validator for each DTO
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        // ✅ Integrate with Value Object validation
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("価格は0より大きい値を入力してください");
            
        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKUは必須です")
            .Must(BeValidSKU)
            .WithMessage("SKU形式が正しくありません");
            
        RuleFor(x => x.InitialStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("初期在庫は0以上の値を入力してください");
    }
    
    // ✅ REQUIRED: Leverage Value Object validation
    private bool BeValidSKU(string sku)
    {
        try
        {
            ProductSKU.Create(sku);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

#### ✅ REQUIRED Controller Integration
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IValidator<CreateProductDto> _validator;
    private readonly IProductService _productService;
    
    public ProductsController(
        IValidator<CreateProductDto> validator,
        IProductService productService)
    {
        _validator = validator;
        _productService = productService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto)
    {
        // ✅ REQUIRED: Validate using FluentValidation
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        // ✅ Continue with business logic
        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}
```

#### ✅ REQUIRED DI Configuration
```csharp
// ✅ REQUIRED: Register FluentValidation in Program.cs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
```

### FluentValidation Testing Requirements
**MANDATORY: ALL validators must have comprehensive tests**

#### ✅ REQUIRED Validator Test Pattern
```csharp
public class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator;
    
    public CreateProductDtoValidatorTests()
    {
        _validator = new CreateProductDtoValidator();
    }
    
    [Fact]
    public void Should_Have_Error_When_Price_Is_Zero()
    {
        // Arrange
        var dto = new CreateProductDto { Price = 0 };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateProductDto.Price));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("INVALID-SKU!")]
    [InlineData("A")]
    public void Should_Have_Error_When_SKU_Is_Invalid(string invalidSku)
    {
        // Arrange
        var dto = new CreateProductDto { SKU = invalidSku };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateProductDto.SKU));
    }
}
```

### FluentValidation with Value Objects Integration
**CRITICAL: Value Objects and FluentValidation must work together**

#### Value Object Validation Pattern
```csharp
// ✅ In Value Object: Core validation logic
public static ProductSKU Create(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("SKU cannot be null or empty");
        
    var normalized = value.Trim().ToUpperInvariant();
    if (!SkuPattern.IsMatch(normalized))
        throw new ArgumentException("SKU format is invalid");
        
    return new ProductSKU(normalized);
}

// ✅ In FluentValidation: Leverage Value Object validation
private bool BeValidSKU(string sku)
{
    try
    {
        ProductSKU.Create(sku);
        return true;
    }
    catch
    {
        return false;
    }
}
```

### Value Object Examples in This Project
```csharp
// Money value object - 通貨付き価格
var price = Money.FromDecimal(1000, "JPY");
var total = price.Multiply(5);
var isAffordable = price <= Money.FromDecimal(500);

// ProductSKU value object - SKU検証・正規化
var sku = ProductSKU.Create("prod-001"); // "PROD-001"に正規化
var hasPrefix = sku.HasPrefix("PROD");

// StockQuantity value object - 在庫ビジネスルール
var stock = StockQuantity.Create(10);
var newStock = stock.Deduct(3); // 在庫減算
var isSufficient = stock.IsSufficient(5); // 充足性チェック
```

## 🏛️ RICH DOMAIN MODEL PATTERNS

### Entity Implementation Requirements
```csharp
// ✅ REQUIRED Rich Domain Entity Pattern
public class {EntityName}
{
    // ✅ Private constructor to enforce factory method
    private {EntityName}() { /* ... */ }
    
    // ✅ Factory method using Value Objects
    public static {EntityName} Create(/* parameters using value objects */)
    {
        var entity = new {EntityName}();
        // Validation and initialization
        // Use value objects for type safety
        return entity;
    }
    
    // ✅ Properties using Value Objects (not primitives)
    public Money Price { get; private set; }
    public ProductSKU SKU { get; private set; }
    public StockQuantity Stock { get; private set; }
    
    // ✅ Business methods (rich domain model)
    public void SomeBusinessAction(/* parameters */)
    {
        // Business logic here
        // Use value object methods
        // Maintain aggregate consistency
    }
    
    // ✅ Business validation methods
    public bool CanPerformAction() { /* ... */ }
    public void EnsureInvariant() { /* ... */ }
}
```

## Code Style and Conventions

### DDD Naming Conventions
- **Entities**: `{BusinessConcept}.cs` (e.g., `Product.cs`, `Order.cs`)
- **Value Objects**: `{BusinessValue}.cs` (e.g., `Money.cs`, `ProductSKU.cs`)
- **Services**: `I{Entity}Service.cs` and `{Entity}Service.cs`
- **Repositories**: `I{Entity}Repository.cs` and `{Entity}Repository.cs`
- **DTOs**: `{Action}{Entity}Dto.cs` (e.g., `CreateProductDto.cs`)

### DDD File Organization
```
src/CopilotSample.Api/
├── Domain/             # ドメイン層（最重要・純粋）
│   ├── Entities/       # エンティティ（ビジネスロジック）
│   └── Values/         # 値オブジェクト（型安全性）
├── Application/        # アプリケーション層（ユースケース調整）
│   ├── Services/       # アプリケーションサービス
│   ├── Interfaces/     # 依存性逆転
│   └── DTOs/           # データ転送オブジェクト
├── Infrastructure/     # インフラ層（外部システム）
│   ├── Data/           # Entity Framework
│   └── Repositories/   # データアクセス実装
└── Controllers/        # プレゼンテーション層（HTTP）
```

## 🧪 MANDATORY TESTING REQUIREMENTS

### Test Creation is REQUIRED for DDD
**For EVERY new domain concept, value object, or business rule, you MUST:**
1. Create corresponding test file first
2. Write comprehensive test cases using **InMemory Database ONLY**
3. Test Value Object business rules and validation
4. Test Entity business methods and invariants
5. Run tests with `dotnet test` to verify they work
6. Include tests in your implementation plan
7. **NEVER use mocks - always use real objects with InMemory DB**

### DDD Test File Naming
- Value Object tests: `{ValueObject}Tests.cs`
- Entity tests: `{Entity}Tests.cs` (focus on business methods)
- Repository tests: `{Entity}RepositoryTests.cs`
- Service tests: `{Entity}ServiceTests.cs`
- **FluentValidation tests: `{DTO}ValidatorTests.cs`**
- All test files go in `tests/CopilotSample.Tests/` directory

### Value Object Testing Template
```csharp
public class MoneyTests
{
    [Theory]
    [InlineData(-1)]    // Negative amount
    [InlineData(-0.01)] // Negative decimal
    public void Create_Should_ThrowException_When_AmountIsNegative(decimal invalidAmount)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.FromDecimal(invalidAmount));
    }
    
    [Fact]
    public void Add_Should_CombineAmounts_When_SameCurrency()
    {
        // Arrange
        var money1 = Money.FromDecimal(100, "JPY");
        var money2 = Money.FromDecimal(200, "JPY");
        
        // Act
        var result = money1.Add(money2);
        
        // Assert
        Assert.Equal(300m, result.Amount);
        Assert.Equal("JPY", result.Currency);
    }
}
```

### Rich Domain Model Testing Template
```csharp
public class ProductTests
{
    [Fact]
    public void Create_Should_CreateProduct_When_ValidValueObjects()
    {
        // Arrange - using Value Objects
        var price = Money.FromDecimal(1000);
        var sku = ProductSKU.Create("PROD-001");
        var stock = StockQuantity.Create(10);
        
        // Act - using factory method
        var product = Product.Create("Test Product", "Description", 
                                     price, sku, 1, stock);
        
        // Assert - verify value object properties
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(1000m, product.Price.Amount);
        Assert.Equal("PROD-001", product.SKU.Value);
        Assert.Equal(10, product.Stock.Value);
    }
    
    [Fact]
    public void DeductStock_Should_UpdateStockQuantity_When_SufficientStock()
    {
        // Arrange
        var product = Product.Create("Test", "Desc", Money.FromDecimal(100),
                                     ProductSKU.Create("TEST-001"), 1,
                                     StockQuantity.Create(10));
        
        // Act - business method
        product.DeductStock(3);
        
        // Assert - verify business rule
        Assert.Equal(7, product.Stock.Value);
    }
}
```

## Testing Guidelines

### Test Structure - Arrange-Act-Assert Pattern
**MANDATORY: Every single test MUST follow AAA pattern with clear comments:**

#### DDD Test Template (Copy this for every test)
```csharp
[Fact]
public async Task MethodName_Should_ExpectedBehavior_When_Condition()
{
    // Arrange - Set up test data using Value Objects and REAL objects
    var price = Money.FromDecimal(1500);
    var sku = ProductSKU.Create("GAMING-001");
    var stock = StockQuantity.Create(5);
    
    var category = CategoryBuilder.Create().WithName("Electronics");
    var product = ProductBuilder.Create()
        .WithName("Gaming Laptop")
        .WithPrice(price)
        .WithSKU(sku)
        .WithStock(stock)
        .WithCategory(category);

    // Save to REAL InMemory database
    _context.Categories.Add(category);
    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    // Act - Execute the method under test using REAL service/repository
    var result = await _service.SearchProductsAsync("gaming");

    // Assert - Verify the results from REAL database
    Assert.Single(result);
    Assert.Equal("Gaming Laptop", result.First().Name);
    Assert.Equal(1500m, result.First().Price.Amount); // Value Object assertion
}
```

### Test Data Builder Pattern - REQUIRED for DDD
**MANDATORY: Always use builders for test data creation. Support Value Objects.**

#### DDD Builder Template (Copy this for every entity)
```csharp
public class ProductBuilder
{
    private string _name = "Default Product";
    private Money _price = Money.FromDecimal(100.00m);
    private ProductSKU _sku;
    private StockQuantity _stock = StockQuantity.Create(20);
    private int _categoryId = 1;
    private Category? _category = null;

    private ProductBuilder()
    {
        // Generate unique SKU for each instance
        _sku = ProductSKU.Create($"TEST-{Guid.NewGuid().ToString()[..6]}");
    }

    public static ProductBuilder Create() => new ProductBuilder();

    // ✅ REQUIRED: Add fluent methods for each property (Value Objects)
    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public ProductBuilder WithPrice(Money price)
    {
        _price = price;
        return this;
    }
    
    public ProductBuilder WithPrice(decimal amount)
    {
        _price = Money.FromDecimal(amount);
        return this;
    }
    
    public ProductBuilder WithSKU(ProductSKU sku)
    {
        _sku = sku;
        return this;
    }
    
    public ProductBuilder WithStock(StockQuantity stock)
    {
        _stock = stock;
        return this;
    }

    public Product Build()
    {
        // Use domain factory method with Value Objects
        return Product.Create(_name, null, _price, _sku, _categoryId, _stock);
    }

    // ✅ REQUIRED: Implicit conversion for convenience
    public static implicit operator Product(ProductBuilder builder) => builder.Build();
}
```

## Architecture Patterns

### Repository Pattern (DDD Context)
```csharp
// Interface in Application layer (dependency inversion)
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(ProductSKU sku); // Use Value Object
}

// Implementation in Infrastructure layer
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
    
    public async Task<bool> ExistsAsync(ProductSKU sku)
    {
        // EF will convert Value Object to primitive for query
        return await _context.Products
            .AnyAsync(p => p.SKU == sku);
    }
}
```

### Application Service Pattern (DDD Context)
```csharp
// Application Service orchestrates Domain objects
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        // Use Value Objects for validation and type safety
        var price = Money.FromDecimal(dto.Price, dto.Currency ?? "JPY");
        var sku = ProductSKU.Create(dto.SKU);
        var stock = StockQuantity.Create(dto.InitialStock);

        // Check business rules using Value Objects
        if (await _repository.ExistsAsync(sku))
            throw new InvalidOperationException($"SKU '{sku}' already exists");

        // Use domain factory method
        var product = Product.Create(dto.Name, dto.Description, 
                                     price, sku, dto.CategoryId, stock);

        return await _repository.AddAsync(product);
    }
    
    public async Task<bool> IsInStockAsync(int productId, int quantity)
    {
        var product = await _repository.GetByIdAsync(productId);
        if (product == null) return false;
        
        // Delegate to domain method (rich domain model)
        return product.IsInStock(quantity);
    }
}
```

## Entity Framework Guidelines for Value Objects

### DbContext Configuration for Value Objects
```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Value Object conversions
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            // Configure Money Value Object
            entity.Property(p => p.Price)
                .HasConversion(
                    v => v.Amount,          // Store as decimal
                    v => Money.FromDecimal(v, "JPY"), // Load as Money
                    new ValueComparer<Money>(
                        (l, r) => l.Amount == r.Amount && l.Currency == r.Currency,
                        v => HashCode.Combine(v.Amount, v.Currency),
                        v => Money.FromDecimal(v.Amount, v.Currency)))
                .HasPrecision(18, 2);
                
            // Configure ProductSKU Value Object
            entity.Property(p => p.SKU)
                .HasConversion(
                    v => v.Value,           // Store as string
                    v => ProductSKU.Create(v), // Load as ProductSKU
                    new ValueComparer<ProductSKU>(
                        (l, r) => l.Value == r.Value,
                        v => v.Value.GetHashCode(),
                        v => ProductSKU.Create(v.Value)))
                .IsRequired()
                .HasMaxLength(50);
                
            entity.HasIndex(p => p.SKU).IsUnique();
            
            // Configure StockQuantity Value Object
            entity.Property(p => p.Stock)
                .HasConversion(
                    v => v.Value,           // Store as int
                    v => StockQuantity.Create(v), // Load as StockQuantity
                    new ValueComparer<StockQuantity>(
                        (l, r) => l.Value == r.Value,
                        v => v.Value.GetHashCode(),
                        v => StockQuantity.Create(v.Value)));
        });
    }
}
```

### Query Patterns with Value Objects
```csharp
// Good: EF automatically converts Value Objects in queries
var expensiveProducts = await _context.Products
    .Where(p => p.Price.Amount > 1000)  // Uses Money Value Object
    .ToListAsync();

// Good: Search by SKU Value Object
var product = await _context.Products
    .FirstOrDefaultAsync(p => p.SKU == ProductSKU.Create("PROD-001"));

// Good: Filter by Stock Value Object
var lowStockProducts = await _context.Products
    .Where(p => p.Stock.Value <= 5)
    .ToListAsync();
```

## Error Handling Patterns

### Domain Layer Exceptions
```csharp
// Value Object validation
public static ProductSKU Create(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("SKU cannot be null or empty", nameof(value));
        
    var normalized = value.Trim().ToUpperInvariant();
    if (!SkuPattern.IsMatch(normalized))
        throw new ArgumentException("SKU format is invalid", nameof(value));
        
    return new ProductSKU(normalized);
}

// Entity business rule validation
public void DeductStock(int quantity)
{
    if (quantity <= 0)
        throw new ArgumentException("Quantity must be positive", nameof(quantity));
        
    if (!Stock.IsSufficient(quantity))
        throw new InvalidOperationException($"Insufficient stock. Available: {Stock.Value}, Required: {quantity}");
        
    Stock = Stock.Deduct(quantity);
    UpdateTimestamp();
}
```

### Application Service Error Handling
```csharp
public async Task<Product> CreateProductAsync(CreateProductDto dto)
{
    try
    {
        // Value Objects will throw if invalid
        var price = Money.FromDecimal(dto.Price);
        var sku = ProductSKU.Create(dto.SKU);
        var stock = StockQuantity.Create(dto.InitialStock);
        
        // Business rule validation
        if (await _repository.ExistsAsync(sku))
            throw new InvalidOperationException($"SKU '{sku}' already exists");

        return await _repository.AddAsync(
            Product.Create(dto.Name, dto.Description, price, sku, dto.CategoryId, stock));
    }
    catch (ArgumentException ex)
    {
        // Value Object validation failed
        throw new ArgumentException($"Invalid input: {ex.Message}", ex);
    }
}
```

## Business Logic Patterns (Rich Domain Model)

### Inventory Management with Value Objects
```csharp
// In Product entity (rich domain model)
public bool IsInStock(int quantity)
{
    // Delegate to Value Object business method
    return Stock.IsSufficient(quantity);
}

public void ProcessSale(int quantity)
{
    // Use Value Object business rules
    if (!Stock.IsSufficient(quantity))
        throw new InvalidOperationException("Insufficient stock");
        
    // Value Object handles the business logic
    Stock = Stock.Deduct(quantity);
    UpdateTimestamp();
}

public Money CalculateInventoryValue()
{
    // Value Objects work together
    return Price.Multiply(Stock.Value);
}

public bool IsLowStock(int threshold = 5)
{
    // Delegate to Value Object
    return Stock.IsLowStock(threshold);
}
```

### Price Management with Money Value Object
```csharp
// In Product entity
public void UpdatePrice(Money newPrice)
{
    if (newPrice.Amount <= 0)
        throw new ArgumentException("Price must be positive");
        
    Price = newPrice;
    UpdateTimestamp();
}

public bool IsPriceInRange(Money minPrice, Money maxPrice)
{
    // Value Objects support comparison operators
    return Price >= minPrice && Price <= maxPrice;
}

public Money CalculateDiscountedPrice(decimal discountPercentage)
{
    if (discountPercentage < 0 || discountPercentage > 100)
        throw new ArgumentException("Discount percentage must be between 0 and 100");
        
    var discountAmount = Price.Multiply(discountPercentage / 100);
    return Price.Subtract(discountAmount);
}
```

## Dependency Injection Setup

### Program.cs Configuration for DDD
```csharp
// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository pattern (Infrastructure layer)
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Application services (Application layer)
builder.Services.AddScoped<IProductService, ProductService>();

// Controllers (Presentation layer)
builder.Services.AddControllers();
```

## Test Planning Comments for DDD

### REQUIRED: Always Plan DDD Tests First
**Before writing any implementation, ALWAYS start with DDD test planning comments:**

```csharp
// TODO: Create comprehensive tests for {ValueObjectName} Value Object
// Value Object scenarios to cover:
// - Should create valid value object when input is correct
// - Should throw exception when input violates business rules
// - Should provide business methods for domain operations
// - Should support equality comparison
// - Should be immutable (no setters)
//
// Test execution plan:
// 1. Create {ValueObject}Tests class 
// 2. Test creation with valid/invalid inputs
// 3. Test business methods and invariants
// 4. Test equality and hash code
// 5. Run 'dotnet test' to verify
```

### DDD Feature Implementation Template
```csharp
// TODO: Implement {FeatureName} following DDD principles
//
// 📁 DDD LAYER-BY-LAYER IMPLEMENTATION PLAN:
//
// STEP 1: DOMAIN LAYER (Core Business Logic)
// File: src/CopilotSample.Api/Domain/Values/{ValueObject}.cs (if needed)
// - Value Object with business rules and validation
// - Immutable structure with factory methods
// - Business operations relevant to this value
//
// File: src/CopilotSample.Api/Domain/Entities/{Entity}.cs
// - Rich domain model with business methods
// - Use Value Objects for type safety
// - Factory method for proper initialization
// - Private setters for encapsulation
//
// STEP 2: APPLICATION LAYER (Use Case Coordination)
// File: src/CopilotSample.Api/Application/DTOs/{Action}{Entity}Dto.cs
// File: src/CopilotSample.Api/Application/Interfaces/I{Entity}Repository.cs
// File: src/CopilotSample.Api/Application/Services/I{Entity}Service.cs
// File: src/CopilotSample.Api/Application/Services/{Entity}Service.cs
// - Orchestrate Domain objects
// - Handle cross-cutting concerns
// - Manage transactions
//
// STEP 3: INFRASTRUCTURE LAYER (External Systems)
// File: src/CopilotSample.Api/Infrastructure/Repositories/{Entity}Repository.cs
// Update: src/CopilotSample.Api/Infrastructure/Data/AppDbContext.cs
// - Implement repository interface
// - Configure Value Object conversions in EF
// - Handle data persistence
//
// STEP 4: PRESENTATION LAYER (HTTP Interface)
// File: src/CopilotSample.Api/Controllers/{Entity}Controller.cs
// Update: src/CopilotSample.Api/Program.cs
// - Thin controllers calling Application Services
// - Proper HTTP status codes and error handling
// - DTO to Domain object mapping
//
// 🧪 TEST STRATEGY (Create FIRST):
// tests/CopilotSample.Tests/Domain/Values/{ValueObject}Tests.cs
// - Test Value Object business rules with various inputs
// - Test creation, validation, and business methods
// - Use Theory/InlineData for multiple scenarios
//
// tests/CopilotSample.Tests/Domain/Entities/{Entity}Tests.cs
// - Test Entity business methods using Value Objects
// - Test aggregate consistency and invariants
// - Test factory methods and validation
//
// tests/CopilotSample.Tests/Repositories/{Entity}RepositoryTests.cs
// - Integration tests with InMemory DB and Value Objects
// - Test EF Value Object conversions
// - Test query patterns with Value Objects
//
// tests/CopilotSample.Tests/Services/{Entity}ServiceTests.cs
// - Test use case orchestration with REAL objects
// - Test business flow coordination
// - Test error handling and edge cases
//
// 🔄 VERIFICATION COMMANDS:
// After each layer: dotnet build
// After Value Objects: dotnet test --filter "*{ValueObject}Tests*"
// After Entities: dotnet test --filter "*{Entity}Tests*"
// After Repository: dotnet test --filter "*{Entity}RepositoryTests*"
// After Service: dotnet test --filter "*{Entity}ServiceTests*"
// Final check: dotnet test --verbosity normal
//
// ⚠️ DDD REMINDER: 
// - Domain layer should have NO dependencies on other layers
// - Use Value Objects for type safety and business rules
// - Rich domain model with business logic in entities
// - Application Services orchestrate, don't implement business logic
// - ALL tests use InMemory database with REAL objects - NO MOCKING
```

## Testing Best Practices - MANDATORY COMPLIANCE

### DDD Test Data Setup - REQUIRED
- **MUST** use builders for all test data creation with Value Object support
- **MUST** test Value Object business rules and validation
- **MUST** test Entity business methods with Value Objects
- **MUST** use TestDbContextFactory for InMemory database contexts
- **MUST** use real objects - NEVER use mocks
- **MUST** verify Value Object conversions work correctly with EF

### DDD Test Assertions - REQUIRED
- **MUST** assert on Value Object properties (e.g., `money.Amount`, `sku.Value`)
- **MUST** test Value Object business methods
- **MUST** test Entity business rules and invariants
- **MUST** test both positive and negative scenarios for business rules
- **MUST** include edge cases and error conditions

### DDD Test Coverage Requirements
- **Value Objects**: Test creation, validation, business methods, and equality
- **Entities**: Test business methods, factory methods, and aggregate consistency
- **Repository layer**: Test CRUD operations and EF Value Object conversions using InMemory DB
- **Service layer**: Test use case orchestration and business flow with REAL repositories
- **FluentValidation**: Test all validation rules, error messages, and Value Object integration
- **Minimum**: Every public method must have at least one test using InMemory database
- **Recommended**: Aim for 80%+ code coverage with integration tests

## Code Generation Preferences for DDD

### DDD Test Code (ALWAYS generate first)
When generating DDD test code, ALWAYS include:
- **Value Object tests** for business rules and validation
- **Entity business method tests** using Value Objects
- **FluentValidation tests** for all DTOs with Value Object integration
- **InMemory Database** for all data access testing
- **Real objects** - NEVER mocks
- **AAA pattern** with clear comment sections
- **TestDataBuilder** for all entity creation with Value Object support
- **Specific assertions** on Value Object properties
- **Edge cases** and error scenarios for business rules
- **Theory/InlineData** for Value Object validation scenarios

### DDD Implementation Code (ONLY after tests exist)
When generating DDD implementation code, prefer:
- **Value Objects** for type safety and business rules
- **FluentValidation** for all DTOs with Value Object integration
- **Rich Domain Model** with business logic in entities
- **Factory methods** for proper object initialization
- **Private setters** for encapsulation
- **Dependency injection** for all dependencies
- **Interface-based design** following dependency inversion
- **Entity Framework Value Object conversions** for data persistence
- **Guard clauses** for input validation at boundaries

## 🚨 FINAL REMINDER: DDD + TDD WITH VALUE OBJECTS + FLUENTVALIDATION MANDATORY

**NEVER implement functionality without tests. This is NON-NEGOTIABLE.**
**NEVER use mocks. ALWAYS use InMemory database with real objects.**
**ALWAYS use Value Objects for type safety and business rules.**
**ALWAYS use FluentValidation for all DTOs with Value Object integration.**

### Before ANY code generation:
1. ✅ Plan DDD layers and Value Objects in comments
2. ✅ Create test file and test class structure (InMemory DB)
3. ✅ Write failing tests for Value Objects and business rules using AAA pattern with REAL objects
4. ✅ Write failing tests for FluentValidation rules with Value Object integration
5. ✅ Run `dotnet test` to confirm tests fail
6. ✅ Implement Value Objects with business logic
7. ✅ Implement FluentValidation validators with Value Object integration
8. ✅ Implement Rich Domain Model using Value Objects
9. ✅ Implement Application Services orchestrating Domain objects
10. ✅ Implement Infrastructure with EF Value Object conversions
11. ✅ Run `dotnet test` to confirm tests pass
12. ✅ Refactor if needed while keeping tests green

### DDD Success Criteria
- All tests pass: `dotnet test` shows green
- Value Objects enforce business rules at compile time
- FluentValidation validates DTOs with Value Object integration
- Entities contain business logic (rich domain model)
- Application Services orchestrate without business logic
- EF Value Object conversions work correctly
- Tests follow AAA pattern with InMemory database
- Builders support Value Objects for test data
- NO mocks anywhere in the codebase

This project emphasizes **Domain-Driven Design**, **Value Objects**, **FluentValidation**, **Rich Domain Model**, **integration testing**, **test-driven development**, and clean, testable, maintainable code following established DDD patterns and practices.
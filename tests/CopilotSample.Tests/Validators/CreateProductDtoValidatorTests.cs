using FluentValidation.TestHelper;
using CopilotSample.Api.Application.DTOs;
using CopilotSample.Api.Application.Interfaces;
using CopilotSample.Api.Application.Validators;
using CopilotSample.Api.Domain.Values;

namespace CopilotSample.Tests.Validators;

/// <summary>
/// CreateProductDtoValidator用のテスト
/// FluentValidationのテスト機能を使用してバリデーションルールを検証
/// </summary>
public class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator;
    private readonly Mock<IProductRepository> _mockRepository;

    public CreateProductDtoValidatorTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _validator = new CreateProductDtoValidator(_mockRepository.Object);
    }

    [Fact]
    public async Task 有効なDTOの場合にバリデーションが成功する()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Description = "テスト用の商品説明",
            Price = 1000m,
            SKU = "TEST-001",
            StockQuantity = 10,
            CategoryId = 1
        };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<ProductSKU>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task 商品名が空の場合にバリデーションエラーが発生する(string invalidName)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = invalidName,
            Price = 1000m,
            SKU = "TEST-001",
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    
    [Fact]
    public async Task 商品名がnullの場合にバリデーションエラーが発生する()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = null!,
            Price = 1000m,
            SKU = "TEST-001",
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task 商品名が100文字を超える場合にバリデーションエラーが発生する()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = new string('あ', 101), // 101文字
            Price = 1000m,
            SKU = "TEST-001",
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task 説明が500文字を超える場合にバリデーションエラーが発生する()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Description = new string('あ', 501), // 501文字
            Price = 1000m,
            SKU = "TEST-001",
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task 価格が0以下の場合にバリデーションエラーが発生する(decimal invalidPrice)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Price = invalidPrice,
            SKU = "TEST-001",
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AB")] // 3文字未満
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ")] // 51文字以上
    public async Task SKUが無効な形式の場合にバリデーションエラーが発生する(string invalidSku)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Price = 1000m,
            SKU = invalidSku,
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SKU);
    }

    [Fact]
    public async Task SKUが既に存在する場合にバリデーションエラーが発生する()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Price = 1000m,
            SKU = "EXISTING-SKU",
            StockQuantity = 10,
            CategoryId = 1
        };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<ProductSKU>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SKU)
            .WithErrorMessage("このSKUは既に使用されています");
    }

    [Fact]
    public async Task 在庫数が負の値の場合にバリデーションエラーが発生する()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Price = 1000m,
            SKU = "TEST-001",
            StockQuantity = -1,
            CategoryId = 1
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task カテゴリIDが0以下の場合にバリデーションエラーが発生する(int invalidCategoryId)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Price = 1000m,
            SKU = "TEST-001",
            StockQuantity = 10,
            CategoryId = invalidCategoryId
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Theory]
    [InlineData("PROD-001")]
    [InlineData("ABC-123")]
    [InlineData("TEST123")]
    public async Task 有効なSKU形式の場合にバリデーションが成功する(string validSku)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "テスト商品",
            Price = 1000m,
            SKU = validSku,
            StockQuantity = 10,
            CategoryId = 1
        };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<ProductSKU>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SKU);
    }
}
using FluentValidation;
using FluentValidation.TestHelper;
using CopilotSample.Api.Application.Validators;

namespace CopilotSample.Tests.Validators;

/// <summary>
/// 値オブジェクト用カスタムバリデーターのテスト
/// 再利用可能なバリデーションロジックの動作を検証
/// </summary>
public class ValueObjectValidatorsTests
{
    private readonly TestValidator _validator;

    public ValueObjectValidatorsTests()
    {
        _validator = new TestValidator();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public async Task MustBeValidMoney_有効な価格の場合にバリデーションが成功する(decimal validPrice)
    {
        // Arrange
        var dto = new TestDto { Price = validPrice };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task MustBeValidMoney_無効な価格の場合にバリデーションエラーが発生する(decimal invalidPrice)
    {
        // Arrange
        var dto = new TestDto { Price = invalidPrice };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData("PROD-001")]
    [InlineData("ABC123")]
    [InlineData("TEST-PRODUCT")]
    public async Task MustBeValidSKU_有効なSKUの場合にバリデーションが成功する(string validSku)
    {
        // Arrange
        var dto = new TestDto { SKU = validSku };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SKU);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AB")] // 3文字未満
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ")] // 51文字以上
    public async Task MustBeValidSKU_無効なSKUの場合にバリデーションエラーが発生する(string invalidSku)
    {
        // Arrange
        var dto = new TestDto { SKU = invalidSku };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SKU);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public async Task MustBeValidStockQuantity_有効な在庫数の場合にバリデーションが成功する(int validStock)
    {
        // Arrange
        var dto = new TestDto { StockQuantity = validStock };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task MustBeValidStockQuantity_負の在庫数の場合にバリデーションエラーが発生する(int invalidStock)
    {
        // Arrange
        var dto = new TestDto { StockQuantity = invalidStock };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Theory]
    [InlineData("テスト商品")]
    [InlineData("Product Name")]
    [InlineData("商品-123")]
    public async Task MustBeValidProductName_有効な商品名の場合にバリデーションが成功する(string validName)
    {
        // Arrange
        var dto = new TestDto { Name = validName };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("商品<script>")]
    [InlineData("商品\"quote\"")]
    public async Task MustBeValidProductName_無効な商品名の場合にバリデーションエラーが発生する(string invalidName)
    {
        // Arrange
        var dto = new TestDto { Name = invalidName };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task MustBeValidProductName_100文字を超える商品名の場合にバリデーションエラーが発生する()
    {
        // Arrange
        string longName = new('あ', 101);
        var dto = new TestDto { Name = longName };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("通常の説明")]
    [InlineData("Normal description")]
    [InlineData(null)]
    public async Task MustBeValidProductDescription_有効な説明の場合にバリデーションが成功する(string? validDescription)
    {
        // Arrange
        var dto = new TestDto { Description = validDescription };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("説明<script>")]
    [InlineData("説明\"quote\"")]
    public async Task MustBeValidProductDescription_特殊文字を含む説明の場合にバリデーションエラーが発生する(string invalidDescription)
    {
        // Arrange
        var dto = new TestDto { Description = invalidDescription };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task MustBeValidProductDescription_500文字を超える説明の場合にバリデーションエラーが発生する()
    {
        // Arrange
        string longDescription = new('あ', 501);
        var dto = new TestDto { Description = longDescription };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    /// テスト用のバリデータークラス
    /// カスタムバリデーターの動作をテストするために使用
    /// </summary>
    private class TestValidator : AbstractValidator<TestDto>
    {
        public TestValidator()
        {
            RuleFor(x => x.Price).MustBeValidMoney();
            RuleFor(x => x.SKU).MustBeValidSKU();
            RuleFor(x => x.StockQuantity).MustBeValidStockQuantity();
            RuleFor(x => x.Name).MustBeValidProductName();
            RuleFor(x => x.Description).MustBeValidProductDescription();
        }
    }

    /// <summary>
    /// テスト用のDTOクラス
    /// </summary>
    private class TestDto
    {
        public decimal Price { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
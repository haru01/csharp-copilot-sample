using FluentValidation.TestHelper;
using CopilotSample.Api.Application.DTOs;
using CopilotSample.Api.Application.Validators;

namespace CopilotSample.Tests.Validators;

/// <summary>
/// UpdateProductDtoValidator用のテスト
/// 部分更新対応のバリデーションルールを検証
/// </summary>
public class UpdateProductDtoValidatorTests
{
    private readonly UpdateProductDtoValidator _validator;

    public UpdateProductDtoValidatorTests()
    {
        _validator = new UpdateProductDtoValidator();
    }

    [Fact]
    public async Task 有効な部分更新DTOの場合にバリデーションが成功する()
    {
        // Arrange - 名前のみの更新
        var dto = new UpdateProductDto
        {
            Name = "更新された商品名"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task 全プロパティが設定された場合にバリデーションが成功する()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = "更新された商品名",
            Description = "更新された説明",
            Price = 2000m,
            StockQuantity = 20,
            CategoryId = 2
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task 空のDTOの場合にバリデーションエラーが発生する()
    {
        // Arrange - すべてのプロパティがnull
        var dto = new UpdateProductDto();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("更新内容が指定されていません。少なくとも1つのプロパティを設定してください");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task 名前が空文字の場合にバリデーションエラーが発生する(string invalidName)
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = invalidName
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task 名前が100文字を超える場合にバリデーションエラーが発生する()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = new string('あ', 101) // 101文字
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
        var dto = new UpdateProductDto
        {
            Description = new string('あ', 501) // 501文字
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
        var dto = new UpdateProductDto
        {
            Price = invalidPrice
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public async Task 在庫数が負の値の場合にバリデーションエラーが発生する()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            StockQuantity = -1
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
        var dto = new UpdateProductDto
        {
            CategoryId = invalidCategoryId
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public async Task 名前のみ更新の場合にバリデーションが成功する()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = "新しい商品名"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task 価格のみ更新の場合にバリデーションが成功する()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Price = 1500m
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task 在庫数のみ更新の場合にバリデーションが成功する()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            StockQuantity = 50
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StockQuantity);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task 複数プロパティの組み合わせ更新でバリデーションが成功する()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = "更新商品",
            Price = 3000m
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
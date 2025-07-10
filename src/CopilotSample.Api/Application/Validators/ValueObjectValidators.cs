using FluentValidation;
using CopilotSample.Api.Domain.Values;
using CopilotSample.Api.Domain.Validation;

namespace CopilotSample.Api.Application.Validators;

/// <summary>
/// 値オブジェクト用の再利用可能なカスタムバリデーター
/// 複数のDTOで共通利用可能な検証ロジックを提供
/// </summary>
public static class ValueObjectValidators
{
    /// <summary>
    /// Money値オブジェクト用バリデーター（統一ルール使用）
    /// 価格フィールドの検証に使用
    /// </summary>
    public static IRuleBuilderOptions<T, decimal> MustBeValidMoney<T>(
        this IRuleBuilder<T, decimal> ruleBuilder, 
        string? currency = null)
    {
        var validationRule = new MoneyValidationRules.FluentValidationMoneyRule();
        
        return ruleBuilder
            .Must(price => validationRule.Validate(price).IsValid)
            .WithMessage(validationRule.DefaultErrorMessage);
    }
    
    /// <summary>
    /// Nullable Money値オブジェクト用バリデーター（統一ルール使用・更新用）
    /// </summary>
    public static IRuleBuilderOptions<T, decimal?> MustBeValidMoneyWhenSet<T>(
        this IRuleBuilder<T, decimal?> ruleBuilder, 
        string? currency = null)
    {
        var validationRule = new MoneyValidationRules.FluentValidationMoneyRule();
        
        return ruleBuilder
            .Must(price => !price.HasValue || validationRule.Validate(price.Value).IsValid)
            .WithMessage(validationRule.DefaultErrorMessage);
    }
    
    /// <summary>
    /// ProductSKU値オブジェクト用バリデーター
    /// SKUフィールドの検証に使用
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeValidSKU<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("SKUは必須です")
            .Must(IsValidSKU)
            .WithMessage("SKUの形式が正しくありません（英数字・ハイフン、3-50文字）");
    }
    
    /// <summary>
    /// StockQuantity値オブジェクト用バリデーター
    /// 在庫数フィールドの検証に使用
    /// </summary>
    public static IRuleBuilderOptions<T, int> MustBeValidStockQuantity<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0)
            .WithMessage("在庫数は0以上で入力してください")
            .Must(IsValidStockQuantity)
            .WithMessage("在庫数の形式が正しくありません");
    }
    
    /// <summary>
    /// Nullable StockQuantity値オブジェクト用バリデーター（更新用）
    /// </summary>
    public static IRuleBuilderOptions<T, int?> MustBeValidStockQuantityWhenSet<T>(
        this IRuleBuilder<T, int?> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0)
            .WithMessage("在庫数は0以上で入力してください")
            .Must(stock => !stock.HasValue || IsValidStockQuantity(stock.Value))
            .WithMessage("在庫数の形式が正しくありません");
    }
    
    /// <summary>
    /// プロダクト名用バリデーター（共通ビジネスルール）
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeValidProductName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("商品名は必須です")
            .MaximumLength(100)
            .WithMessage("商品名は100文字以内で入力してください")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("商品名は空白のみにはできません")
            .Must(name => !HasSpecialCharacters(name))
            .WithMessage("商品名に使用できない文字が含まれています");
    }
    
    /// <summary>
    /// プロダクト説明用バリデーター（共通ビジネスルール）
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeValidProductDescription<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(500)
            .WithMessage("商品説明は500文字以内で入力してください")
            .Must(desc => desc == null || !HasSpecialCharacters(desc))
            .WithMessage("商品説明に使用できない文字が含まれています");
    }
    
    // プライベートヘルパーメソッド
    
    private static bool IsValidSKU(string? sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;
            
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
    
    private static bool IsValidStockQuantity(int stock)
    {
        try
        {
            StockQuantity.Create(stock);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private static bool HasSpecialCharacters(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return false;
            
        // ビジネスルール：特定の特殊文字を禁止
        char[] forbiddenChars = ['<', '>', '"', '\'', '&'];
        return text.IndexOfAny(forbiddenChars) >= 0;
    }
}
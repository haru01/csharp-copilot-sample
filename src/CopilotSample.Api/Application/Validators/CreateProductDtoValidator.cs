using FluentValidation;
using CopilotSample.Api.Application.DTOs;
using CopilotSample.Api.Application.Interfaces;
using CopilotSample.Api.Domain.Values;

namespace CopilotSample.Api.Application.Validators;

/// <summary>
/// CreateProductDto用のFluentValidationバリデーター
/// 値オブジェクトとの連携により型安全性とビジネスルールの一貫性を確保
/// </summary>
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    private readonly IProductRepository _repository;

    public CreateProductDtoValidator(IProductRepository repository)
    {
        _repository = repository;
        
        // 商品名のバリデーション（カスタムバリデーター使用）
        RuleFor(x => x.Name)
            .MustBeValidProductName();
            
        // 説明のバリデーション（カスタムバリデーター使用）
        RuleFor(x => x.Description)
            .MustBeValidProductDescription();
            
        // 価格のバリデーション（カスタムバリデーター使用）
        RuleFor(x => x.Price)
            .MustBeValidMoney();
            
        // SKUのバリデーション（カスタムバリデーター + 一意性チェック）
        RuleFor(x => x.SKU)
            .MustBeValidSKU()
            .MustAsync(BeUniqueSKU)
            .WithMessage("このSKUは既に使用されています");
            
        // 在庫数のバリデーション（カスタムバリデーター使用）
        RuleFor(x => x.StockQuantity)
            .MustBeValidStockQuantity();
            
        // カテゴリIDのバリデーション
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("カテゴリIDは1以上の値で入力してください");
    }
    
    
    /// <summary>
    /// SKUがデータベース内で一意かチェック（非同期）
    /// </summary>
    private async Task<bool> BeUniqueSKU(string sku, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;
            
        try
        {
            var skuValueObject = ProductSKU.Create(sku);
            var exists = await _repository.ExistsAsync(skuValueObject);
            return !exists; // 存在しない場合はtrue（ユニーク）
        }
        catch
        {
            return false; // 形式が無効な場合もfalse
        }
    }
}
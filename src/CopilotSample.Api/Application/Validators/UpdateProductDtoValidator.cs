using FluentValidation;
using CopilotSample.Api.Application.DTOs;

namespace CopilotSample.Api.Application.Validators;

/// <summary>
/// UpdateProductDto用のFluentValidationバリデーター
/// 部分更新に対応し、値オブジェクトとの連携による検証を実装
/// </summary>
public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        // 空の更新は拒否
        RuleFor(x => x)
            .Must(dto => !dto.IsEmpty())
            .WithMessage("更新内容が指定されていません。少なくとも1つのプロパティを設定してください");
            
        // 商品名のバリデーション（設定されている場合のみ）
        When(x => x.Name != null, () => {
            RuleFor(x => x.Name!)
                .MustBeValidProductName();
        });
            
        // 説明のバリデーション（設定されている場合のみ）
        When(x => x.Description != null, () => {
            RuleFor(x => x.Description)
                .MustBeValidProductDescription();
        });
            
        // 価格のバリデーション（カスタムバリデーター使用）
        RuleFor(x => x.Price)
            .MustBeValidMoneyWhenSet()
            .When(x => x.Price.HasValue);
            
        // 在庫数のバリデーション（カスタムバリデーター使用）
        RuleFor(x => x.StockQuantity)
            .MustBeValidStockQuantityWhenSet()
            .When(x => x.StockQuantity.HasValue);
            
        // カテゴリIDのバリデーション（設定されている場合のみ）
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("カテゴリIDは1以上の値で入力してください")
            .When(x => x.CategoryId.HasValue);
    }
    
}
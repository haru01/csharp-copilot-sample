namespace CopilotSample.Api.Application.DTOs;

/// <summary>
/// 商品更新用DTO
/// CreateProductDtoとは異なり、部分更新に対応し、SKUは変更不可とする
/// FluentValidationのUpdateProductDtoValidatorで検証を行う
/// </summary>
public class UpdateProductDto
{
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public decimal? Price { get; set; }
    
    // SKUは更新時に変更不可（ビジネスルール）
    // 必要な場合は別の商品として新規作成する
    
    public int? StockQuantity { get; set; }
    
    public int? CategoryId { get; set; }
    
    /// <summary>
    /// すべてのプロパティがnullかどうかをチェック
    /// </summary>
    public bool IsEmpty()
    {
        return Name == null && 
               Description == null && 
               Price == null && 
               StockQuantity == null && 
               CategoryId == null;
    }
}
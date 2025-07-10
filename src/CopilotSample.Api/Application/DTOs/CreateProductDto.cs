namespace CopilotSample.Api.Application.DTOs;

/// <summary>
/// 商品作成用DTO
/// FluentValidationのCreateProductDtoValidatorで検証を行う
/// 値オブジェクトとの連携により型安全性とビジネスルールの一貫性を確保
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public decimal Price { get; set; }
    
    public string SKU { get; set; } = string.Empty;
    
    public int StockQuantity { get; set; }
    
    public int CategoryId { get; set; }
}
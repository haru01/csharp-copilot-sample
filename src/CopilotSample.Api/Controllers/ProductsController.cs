using Microsoft.AspNetCore.Mvc;
using CopilotSample.Api.Application.Services;
using CopilotSample.Api.Application.DTOs;
using CopilotSample.Api.Domain.Entities;

namespace CopilotSample.Api.Controllers;

// TODO: REST API controller for products
// - GET /api/products - Get all products
// - GET /api/products/{id} - Get product by ID
// - POST /api/products - Create new product
// - PUT /api/products/{id} - Update product
// - DELETE /api/products/{id} - Delete product
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    
    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }
    
    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found");
        }
        
        return Ok(product);
    }
    
    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductDto dto)
    {
        // ModelValidationFilterによりバリデーションエラーは自動処理される
        try
        {
            var createdProduct = await _productService.CreateProductAsync(dto);
            
            return CreatedAtAction(
                nameof(GetProduct), 
                new { id = createdProduct.Id }, 
                createdProduct);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    {
        // ModelValidationFilterによりバリデーションエラーは自動処理される
        try
        {
            var updated = await _productService.UpdateProductAsync(id, dto);
            
            if (!updated)
            {
                return NotFound($"Product with ID {id} not found");
            }
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteProductAsync(id);
        
        if (!deleted)
        {
            return NotFound($"Product with ID {id} not found");
        }
        
        return NoContent();
    }
}
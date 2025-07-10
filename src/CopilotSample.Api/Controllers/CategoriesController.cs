using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CopilotSample.Api.Application.DTOs;
using CopilotSample.Api.Application.Services;
using CopilotSample.Api.Domain.Entities;

namespace CopilotSample.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _service;
        public CategoriesController(CategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            try
            {
                var category = await _service.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(CreateCategory), new { id = category.Id }, category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}

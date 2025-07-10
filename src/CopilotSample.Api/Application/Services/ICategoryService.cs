using System.Threading.Tasks;
using CopilotSample.Api.Domain.Entities;

namespace CopilotSample.Api.Application.Services
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(CopilotSample.Api.Application.DTOs.CreateCategoryDto dto);
    }
}

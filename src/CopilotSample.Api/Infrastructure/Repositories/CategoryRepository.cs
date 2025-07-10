using System.Threading.Tasks;
using CopilotSample.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CopilotSample.Api.Infrastructure.Data;

namespace CopilotSample.Api.Infrastructure.Repositories
{
    public class CategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name.Value == name);
        }
    }
}

using CopilotSample.Api.Domain.Values;

namespace CopilotSample.Api.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public CategoryName Name { get; set; } = new CategoryName("");
        public string? Description { get; set; }
        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
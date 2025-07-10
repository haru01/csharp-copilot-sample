using CopilotSample.Api.Domain.Entities;

namespace CopilotSample.Tests.Helpers;

// Builder pattern for creating Category test data with sensible defaults
public class CategoryBuilder
{
    private string _name = "Default Category";
    private string? _description = "Default category description";

    public static CategoryBuilder Create() => new CategoryBuilder();

    public CategoryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CategoryBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public Category Build()
    {
        return new Category
        {
            Name = _name,
            Description = _description
        };
    }

    // Implicit conversion for convenience
    public static implicit operator Category(CategoryBuilder builder) => builder.Build();
}
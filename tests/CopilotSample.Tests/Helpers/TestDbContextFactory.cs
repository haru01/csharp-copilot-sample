using Microsoft.EntityFrameworkCore;
using CopilotSample.Api.Infrastructure.Data;
using CopilotSample.Api.Domain.Entities;

namespace CopilotSample.Tests.Helpers;

// TODO: Factory to create in-memory database contexts for testing
// - Each test gets a unique database name
// - Seed test data if needed
public class TestDbContextFactory
{
    public static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        var context = new AppDbContext(options);
        
        // No seed data - all tests should arrange their own data for clarity and independence
        
        return context;
    }
    
}
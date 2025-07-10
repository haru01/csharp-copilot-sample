using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using CopilotSample.Api.Infrastructure.Data;
using CopilotSample.Api.Infrastructure.Repositories;
using CopilotSample.Api.Application.Interfaces;
using CopilotSample.Api.Application.Services;
using CopilotSample.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=copilot_sample.db"));

// Add controllers with validation filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CopilotSample.Api.Filters.ModelValidationFilter>();
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register repository and service
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IValidationService, ValidationService>();

// Category用リポジトリ・サービス登録
builder.Services.AddScoped<CopilotSample.Api.Infrastructure.Repositories.CategoryRepository>();
builder.Services.AddScoped<CopilotSample.Api.Application.Services.CategoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// バリデーション例外ミドルウェアを追加（早い段階で処理）
app.UseMiddleware<ValidationExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Apply migrations automatically in development
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

// Map controllers
app.MapControllers();

app.Run();

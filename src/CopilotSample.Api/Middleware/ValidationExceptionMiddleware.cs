using System.Net;
using System.Text.Json;
using FluentValidation;

namespace CopilotSample.Api.Middleware;

/// <summary>
/// FluentValidation例外を統一されたAPIレスポンス形式に変換するミドルウェア
/// バリデーションエラーを構造化されたJSONで返す
/// </summary>
public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger;

    public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("Validation exception occurred: {Message}", ex.Message);
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var response = new ValidationErrorResponse
        {
            Message = "One or more validation errors occurred.",
            Errors = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                )
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = "An internal error occurred.",
            Details = ex.Message // 開発環境でのみ詳細を表示することを検討
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// バリデーションエラーレスポンス用のデータ構造
/// </summary>
public class ValidationErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]> Errors { get; set; } = new();
}

/// <summary>
/// 一般的なエラーレスポンス用のデータ構造
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
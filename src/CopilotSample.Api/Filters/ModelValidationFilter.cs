using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CopilotSample.Api.Middleware;

namespace CopilotSample.Api.Filters;

/// <summary>
/// ModelStateの検証結果を統一されたレスポンス形式で返すフィルター
/// FluentValidationと連携してバリデーションエラーを処理
/// </summary>
public class ModelValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var response = new ValidationErrorResponse
            {
                Message = "One or more validation errors occurred.",
                Errors = context.ModelState
                    .Where(ms => ms.Value?.Errors.Count > 0)
                    .ToDictionary(
                        ms => ToCamelCase(ms.Key),
                        ms => ms.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    )
            };

            context.Result = new BadRequestObjectResult(response);
        }

        base.OnActionExecuting(context);
    }

    /// <summary>
    /// プロパティ名をキャメルケースに変換
    /// フロントエンドのJavaScript命名規則に合わせる
    /// </summary>
    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input[1..];
    }
}
using FluentValidation.Results;

namespace CopilotSample.Api.Application.Services;

/// <summary>
/// Application層での明示的バリデーション用サービスインターフェース
/// コントローラーでの自動バリデーション以外に、
/// Application層でのビジネスロジック内でのバリデーションが必要な場合に使用
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// オブジェクトを明示的にバリデーションする
    /// </summary>
    /// <typeparam name="T">バリデーション対象の型</typeparam>
    /// <param name="instance">バリデーション対象インスタンス</param>
    /// <returns>バリデーション結果</returns>
    Task<ValidationResult> ValidateAsync<T>(T instance);
    
    /// <summary>
    /// バリデーション結果を例外に変換する
    /// </summary>
    /// <param name="validationResult">バリデーション結果</param>
    /// <exception cref="ValidationException">バリデーションエラーがある場合</exception>
    void ThrowIfInvalid(ValidationResult validationResult);
}
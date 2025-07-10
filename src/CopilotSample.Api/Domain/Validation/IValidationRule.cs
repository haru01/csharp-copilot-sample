namespace CopilotSample.Api.Domain.Validation;

/// <summary>
/// 値オブジェクトとFluentValidationで共有するバリデーションルールのインターフェース
/// DRY原則に従い、バリデーションロジックの重複を防ぐ
/// </summary>
/// <typeparam name="T">バリデーション対象の型</typeparam>
public interface IValidationRule<T>
{
    /// <summary>
    /// バリデーションを実行する
    /// </summary>
    /// <param name="value">検証対象の値</param>
    /// <returns>バリデーション結果</returns>
    ValidationResult Validate(T value);
    
    /// <summary>
    /// エラーメッセージキー（国際化対応）
    /// </summary>
    string ErrorMessageKey { get; }
    
    /// <summary>
    /// デフォルトエラーメッセージ（国際化ファイルが無い場合のフォールバック）
    /// </summary>
    string DefaultErrorMessage { get; }
}

/// <summary>
/// バリデーション結果を表すクラス
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? ErrorCode { get; private set; }

    private ValidationResult(bool isValid, string? errorMessage = null, string? errorCode = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static ValidationResult Success() => new(true);
    
    public static ValidationResult Failure(string errorMessage, string? errorCode = null) 
        => new(false, errorMessage, errorCode);
}
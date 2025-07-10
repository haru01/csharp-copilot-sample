using FluentValidation;
using FluentValidation.Results;

namespace CopilotSample.Api.Application.Services;

/// <summary>
/// Application層での明示的バリデーション用サービス実装
/// FluentValidationを使用してオブジェクトをバリデーションし、
/// ビジネスロジック内での検証に使用する
/// </summary>
public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;
    
    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// オブジェクトを明示的にバリデーションする
    /// </summary>
    public async Task<ValidationResult> ValidateAsync<T>(T instance)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        
        if (validator == null)
        {
            // バリデーターが登録されていない場合は成功とみなす
            return new ValidationResult();
        }
        
        return await validator.ValidateAsync(instance);
    }
    
    /// <summary>
    /// バリデーション結果を例外に変換する
    /// </summary>
    public void ThrowIfInvalid(ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
    }
}
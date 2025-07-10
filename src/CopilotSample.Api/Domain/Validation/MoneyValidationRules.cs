namespace CopilotSample.Api.Domain.Validation;

/// <summary>
/// Money値オブジェクト用の統一バリデーションルール
/// 値オブジェクトとFluentValidationで共有される
/// </summary>
public static class MoneyValidationRules
{
    /// <summary>
    /// 正の値チェックルール（Zeroを許可）
    /// </summary>
    public class NonNegativeAmountRule : IValidationRule<decimal>
    {
        public string ErrorMessageKey => "Money.Amount.MustBeNonNegative";
        public string DefaultErrorMessage => "価格は0以上の値で入力してください";

        public ValidationResult Validate(decimal value)
        {
            if (value < 0)
            {
                return ValidationResult.Failure(DefaultErrorMessage, "MONEY_001");
            }
            
            return ValidationResult.Success();
        }
    }
    
    /// <summary>
    /// 正の値チェックルール（Zeroを許可しない）- FluentValidation用
    /// </summary>
    public class PositiveAmountRule : IValidationRule<decimal>
    {
        public string ErrorMessageKey => "Money.Amount.MustBePositive";
        public string DefaultErrorMessage => "価格は0より大きい値で入力してください";

        public ValidationResult Validate(decimal value)
        {
            if (value <= 0)
            {
                return ValidationResult.Failure(DefaultErrorMessage, "MONEY_002");
            }
            
            return ValidationResult.Success();
        }
    }

    /// <summary>
    /// 妥当な範囲チェックルール（例：0円～999,999,999円）
    /// </summary>
    public class ReasonableAmountRule : IValidationRule<decimal>
    {
        private const decimal MaxAmount = 999_999_999m;
        private const decimal MinAmount = 0m;

        public string ErrorMessageKey => "Money.Amount.OutOfRange";
        public string DefaultErrorMessage => $"価格は{MinAmount:N0}円～{MaxAmount:N0}円の範囲で入力してください";

        public ValidationResult Validate(decimal value)
        {
            if (value < MinAmount || value > MaxAmount)
            {
                return ValidationResult.Failure(DefaultErrorMessage, "MONEY_003");
            }
            
            return ValidationResult.Success();
        }
    }

    /// <summary>
    /// 小数点以下桁数チェックルール（通貨による）
    /// </summary>
    public class DecimalPrecisionRule : IValidationRule<(decimal Amount, string Currency)>
    {
        public string ErrorMessageKey => "Money.Amount.InvalidPrecision";
        public string DefaultErrorMessage => "価格の小数点以下桁数が正しくありません";

        public ValidationResult Validate((decimal Amount, string Currency) value)
        {
            var (amount, currency) = value;
            
            var maxDecimalPlaces = currency.ToUpperInvariant() switch
            {
                "JPY" => 0,  // 日本円は整数のみ
                "USD" => 2,  // 米ドルは2桁まで
                "EUR" => 2,  // ユーロは2桁まで
                _ => 2       // デフォルトは2桁
            };

            var actualDecimalPlaces = GetDecimalPlaces(amount);
            
            if (actualDecimalPlaces > maxDecimalPlaces)
            {
                return ValidationResult.Failure(
                    $"{currency}は小数点以下{maxDecimalPlaces}桁まで入力可能です", 
                    "MONEY_004");
            }
            
            return ValidationResult.Success();
        }

        private static int GetDecimalPlaces(decimal value)
        {
            var bits = decimal.GetBits(value);
            return (bits[3] >> 16) & 0xFF;
        }
    }

    /// <summary>
    /// 通貨コード妥当性チェックルール
    /// </summary>
    public class ValidCurrencyRule : IValidationRule<string>
    {
        private static readonly HashSet<string> ValidCurrencies = new(StringComparer.OrdinalIgnoreCase)
        {
            "JPY", "USD", "EUR", "GBP", "AUD", "CAD", "CHF", "CNY", "KRW"
        };

        public string ErrorMessageKey => "Money.Currency.Invalid";
        public string DefaultErrorMessage => "対応していない通貨コードです";

        public ValidationResult Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure("通貨コードは必須です", "MONEY_005");
            }

            if (!ValidCurrencies.Contains(value.Trim()))
            {
                return ValidationResult.Failure(DefaultErrorMessage, "MONEY_006");
            }
            
            return ValidationResult.Success();
        }
    }

    /// <summary>
    /// すべてのMoneyバリデーションルールを組み合わせた複合ルール（値オブジェクト用）
    /// </summary>
    public class CompositeMoneyRule : IValidationRule<(decimal Amount, string Currency)>
    {
        private readonly NonNegativeAmountRule _nonNegativeRule = new();
        private readonly ReasonableAmountRule _rangeRule = new();
        private readonly DecimalPrecisionRule _precisionRule = new();
        private readonly ValidCurrencyRule _currencyRule = new();

        public string ErrorMessageKey => "Money.Composite.Invalid";
        public string DefaultErrorMessage => "価格の形式が正しくありません";

        public ValidationResult Validate((decimal Amount, string Currency) value)
        {
            var (amount, currency) = value;

            // 通貨チェック
            var currencyResult = _currencyRule.Validate(currency);
            if (!currencyResult.IsValid)
                return currencyResult;

            // 非負の値チェック
            var nonNegativeResult = _nonNegativeRule.Validate(amount);
            if (!nonNegativeResult.IsValid)
                return nonNegativeResult;

            // 範囲チェック
            var rangeResult = _rangeRule.Validate(amount);
            if (!rangeResult.IsValid)
                return rangeResult;

            // 小数点桁数チェック
            var precisionResult = _precisionRule.Validate(value);
            if (!precisionResult.IsValid)
                return precisionResult;

            return ValidationResult.Success();
        }
    }

    /// <summary>
    /// FluentValidation用の複合ルール（正の値必須）
    /// </summary>
    public class FluentValidationMoneyRule : IValidationRule<decimal>
    {
        private readonly PositiveAmountRule _positiveRule = new();
        private readonly ReasonableAmountRule _rangeRule = new();

        public string ErrorMessageKey => "Money.FluentValidation.Invalid";
        public string DefaultErrorMessage => "価格の形式が正しくありません";

        public ValidationResult Validate(decimal value)
        {
            // 正の値チェック（FluentValidationでは0は許可しない）
            var positiveResult = _positiveRule.Validate(value);
            if (!positiveResult.IsValid)
                return positiveResult;

            // 範囲チェック
            var rangeResult = _rangeRule.Validate(value);
            if (!rangeResult.IsValid)
                return rangeResult;

            return ValidationResult.Success();
        }
    }
}
using CopilotSample.Api.Domain.Validation;

namespace CopilotSample.Api.Domain.Values;

/// <summary>
/// Money value object representing monetary amounts with currency
/// Ensures type safety and encapsulates money-related business rules
/// </summary>
public readonly record struct Money(decimal Amount, string Currency = "JPY")
{
    public static Money Zero => new(0);  // Zeroは例外的に許可
    
    /// <summary>
    /// Creates Money from decimal amount with specified currency
    /// 統一バリデーションルールを使用してバリデーションを実行
    /// </summary>
    public static Money FromDecimal(decimal amount, string currency = "JPY")
    {
        // 統一バリデーションルールを使用
        var validationRule = new MoneyValidationRules.CompositeMoneyRule();
        var result = validationRule.Validate((amount, currency));
        
        if (!result.IsValid)
        {
            throw new ArgumentException(result.ErrorMessage ?? "Invalid money amount", nameof(amount));
        }
        
        return new Money(amount, currency.ToUpperInvariant());
    }
    
    /// <summary>
    /// Creates Money from integer amount (useful for whole number prices)
    /// </summary>
    public static Money FromInt(int amount, string currency = "JPY")
    {
        return FromDecimal(amount, currency);
    }
    
    /// <summary>
    /// Adds two Money values (must be same currency)
    /// </summary>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");
        
        return new Money(Amount + other.Amount, Currency);
    }
    
    /// <summary>
    /// Subtracts Money values (must be same currency)
    /// </summary>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");
        
        var result = Amount - other.Amount;
        if (result < 0)
            throw new InvalidOperationException("Subtraction would result in negative amount");
        
        return new Money(result, Currency);
    }
    
    /// <summary>
    /// Multiplies Money by quantity (for calculating total prices)
    /// </summary>
    public Money Multiply(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
        
        return new Money(Amount * quantity, Currency);
    }
    
    /// <summary>
    /// Multiplies Money by decimal factor
    /// </summary>
    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Factor cannot be negative", nameof(factor));
        
        return new Money(Amount * factor, Currency);
    }
    
    /// <summary>
    /// Checks if this Money amount is positive (greater than zero)
    /// </summary>
    public bool IsPositive => Amount > 0;
    
    /// <summary>
    /// Checks if this Money amount is zero
    /// </summary>
    public bool IsZero => Amount == 0;
    
    /// <summary>
    /// Rounds the amount to specified decimal places
    /// </summary>
    public Money Round(int decimals = 2)
    {
        return new Money(Math.Round(Amount, decimals), Currency);
    }
    
    /// <summary>
    /// Formats Money as string with currency symbol
    /// </summary>
    public override string ToString()
    {
        return Currency switch
        {
            "JPY" => $"¥{Amount:N0}",
            "USD" => $"${Amount:N2}",
            "EUR" => $"€{Amount:N2}",
            _ => $"{Amount:N2} {Currency}"
        };
    }
    
    /// <summary>
    /// Implicit conversion from decimal (assumes JPY currency)
    /// </summary>
    public static implicit operator Money(decimal amount) => FromDecimal(amount);
    
    /// <summary>
    /// Explicit conversion to decimal (loses currency information)
    /// </summary>
    public static explicit operator decimal(Money money) => money.Amount;
    
    // Comparison operators
    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
        return left.Amount > right.Amount;
    }
    
    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
        return left.Amount < right.Amount;
    }
    
    public static bool operator >=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
        return left.Amount >= right.Amount;
    }
    
    public static bool operator <=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
        return left.Amount <= right.Amount;
    }
    
    // Arithmetic operators
    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, int quantity) => money.Multiply(quantity);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
}
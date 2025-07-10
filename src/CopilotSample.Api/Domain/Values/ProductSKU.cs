using System.Text.RegularExpressions;

namespace CopilotSample.Api.Domain.Values;

/// <summary>
/// ProductSKU value object representing Stock Keeping Unit with validation and normalization
/// Ensures SKU format consistency and business rules
/// </summary>
public readonly record struct ProductSKU(string Value)
{
    // SKU format: alphanumeric characters and hyphens, 3-50 characters
    private static readonly Regex SkuPattern = new(@"^[A-Z0-9\-]{3,50}$", RegexOptions.Compiled);
    
    /// <summary>
    /// Creates a validated ProductSKU from string value
    /// </summary>
    public static ProductSKU Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU cannot be null or empty", nameof(value));
        
        // Normalize: trim whitespace and convert to uppercase
        var normalizedValue = value.Trim().ToUpperInvariant();
        
        if (normalizedValue.Length < 3)
            throw new ArgumentException("SKU must be at least 3 characters long", nameof(value));
        
        if (normalizedValue.Length > 50)
            throw new ArgumentException("SKU cannot exceed 50 characters", nameof(value));
        
        if (!SkuPattern.IsMatch(normalizedValue))
            throw new ArgumentException("SKU can only contain alphanumeric characters and hyphens", nameof(value));
        
        return new ProductSKU(normalizedValue);
    }
    
    /// <summary>
    /// Creates a ProductSKU with prefix and number (e.g., "PROD-001")
    /// </summary>
    public static ProductSKU CreateWithPrefix(string prefix, int number, int paddingLength = 3)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));
        
        if (number < 0)
            throw new ArgumentException("Number cannot be negative", nameof(number));
        
        var paddedNumber = number.ToString().PadLeft(paddingLength, '0');
        var skuValue = $"{prefix.ToUpperInvariant()}-{paddedNumber}";
        
        return Create(skuValue);
    }
    
    /// <summary>
    /// Generates a random SKU with specified prefix
    /// </summary>
    public static ProductSKU GenerateRandom(string prefix = "PROD")
    {
        var random = new Random();
        var number = random.Next(1, 99999);
        return CreateWithPrefix(prefix, number, 5);
    }
    
    /// <summary>
    /// Checks if the SKU has a specific prefix
    /// </summary>
    public bool HasPrefix(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return false;
        
        return Value.StartsWith(prefix.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Extracts the prefix part of the SKU (before first hyphen)
    /// </summary>
    public string GetPrefix()
    {
        var hyphenIndex = Value.IndexOf('-');
        return hyphenIndex > 0 ? Value[..hyphenIndex] : Value;
    }
    
    /// <summary>
    /// Extracts the suffix part of the SKU (after first hyphen)
    /// </summary>
    public string GetSuffix()
    {
        var hyphenIndex = Value.IndexOf('-');
        return hyphenIndex > 0 && hyphenIndex < Value.Length - 1 ? Value[(hyphenIndex + 1)..] : string.Empty;
    }
    
    /// <summary>
    /// Checks if this SKU is valid according to business rules
    /// </summary>
    public bool IsValid()
    {
        try
        {
            Create(Value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Returns the string representation of the SKU
    /// </summary>
    public override string ToString() => Value;
    
    /// <summary>
    /// Implicit conversion from string to ProductSKU
    /// </summary>
    public static implicit operator ProductSKU(string value) => Create(value);
    
    /// <summary>
    /// Implicit conversion from ProductSKU to string
    /// </summary>
    public static implicit operator string(ProductSKU sku) => sku.Value;
    
    /// <summary>
    /// Equality comparison based on normalized value
    /// </summary>
    public bool Equals(ProductSKU other) => 
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    
    /// <summary>
    /// Hash code based on normalized value
    /// </summary>
    public override int GetHashCode() => 
        StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
}
namespace CopilotSample.Api.Domain.Values;

/// <summary>
/// StockQuantity value object representing inventory quantity with business rules
/// Ensures non-negative quantities and encapsulates stock-related operations
/// </summary>
public readonly record struct StockQuantity(int Value)
{
    public static StockQuantity Zero => new(0);
    
    /// <summary>
    /// Creates StockQuantity with validation
    /// </summary>
    public static StockQuantity Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(value));
        
        return new StockQuantity(value);
    }
    
    /// <summary>
    /// Creates StockQuantity from initial stock input
    /// </summary>
    public static StockQuantity FromInitialStock(int initialStock) => Create(initialStock);
    
    /// <summary>
    /// Adds quantity to current stock (for restocking)
    /// </summary>
    public StockQuantity Add(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Cannot add negative quantity", nameof(quantity));
        
        // Check for overflow
        if (Value > int.MaxValue - quantity)
            throw new InvalidOperationException("Stock quantity would exceed maximum value");
        
        return new StockQuantity(Value + quantity);
    }
    
    /// <summary>
    /// Deducts quantity from current stock (for sales/consumption)
    /// </summary>
    public StockQuantity Deduct(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Cannot deduct negative quantity", nameof(quantity));
        
        if (quantity > Value)
            throw new InvalidOperationException($"Insufficient stock. Available: {Value}, Required: {quantity}");
        
        return new StockQuantity(Value - quantity);
    }
    
    /// <summary>
    /// Sets stock to a specific quantity (for inventory adjustments)
    /// </summary>
    public StockQuantity SetTo(int quantity) => Create(quantity);
    
    /// <summary>
    /// Checks if there is sufficient stock for the requested quantity
    /// </summary>
    public bool IsSufficient(int requiredQuantity)
    {
        if (requiredQuantity <= 0)
            return false;
        
        return Value >= requiredQuantity;
    }
    
    /// <summary>
    /// Checks if stock is considered low based on threshold
    /// </summary>
    public bool IsLowStock(int threshold = 5)
    {
        if (threshold < 0)
            throw new ArgumentException("Threshold cannot be negative", nameof(threshold));
        
        return Value <= threshold;
    }
    
    /// <summary>
    /// Checks if stock is at critical level (typically 0 or 1)
    /// </summary>
    public bool IsCriticalStock(int criticalThreshold = 1)
    {
        if (criticalThreshold < 0)
            throw new ArgumentException("Critical threshold cannot be negative", nameof(criticalThreshold));
        
        return Value <= criticalThreshold;
    }
    
    /// <summary>
    /// Checks if stock is completely out (zero)
    /// </summary>
    public bool IsOutOfStock => Value == 0;
    
    /// <summary>
    /// Checks if stock is available (greater than zero)
    /// </summary>
    public bool IsAvailable => Value > 0;
    
    /// <summary>
    /// Calculates how much stock is needed to reach target level
    /// </summary>
    public int GetRestockAmount(int targetLevel)
    {
        if (targetLevel < 0)
            throw new ArgumentException("Target level cannot be negative", nameof(targetLevel));
        
        return Math.Max(0, targetLevel - Value);
    }
    
    /// <summary>
    /// Gets the percentage of stock remaining compared to maximum capacity
    /// </summary>
    public double GetStockPercentage(int maxCapacity)
    {
        if (maxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than zero", nameof(maxCapacity));
        
        return (double)Value / maxCapacity * 100;
    }
    
    /// <summary>
    /// Returns string representation of the stock quantity
    /// </summary>
    public override string ToString() => Value.ToString();
    
    /// <summary>
    /// Formats stock quantity with unit description
    /// </summary>
    public string ToString(string unit) => $"{Value} {unit}";
    
    /// <summary>
    /// Implicit conversion from int to StockQuantity
    /// </summary>
    public static implicit operator StockQuantity(int value) => Create(value);
    
    /// <summary>
    /// Implicit conversion from StockQuantity to int
    /// </summary>
    public static implicit operator int(StockQuantity stock) => stock.Value;
    
    // Comparison operators
    public static bool operator >(StockQuantity left, StockQuantity right) => left.Value > right.Value;
    public static bool operator <(StockQuantity left, StockQuantity right) => left.Value < right.Value;
    public static bool operator >=(StockQuantity left, StockQuantity right) => left.Value >= right.Value;
    public static bool operator <=(StockQuantity left, StockQuantity right) => left.Value <= right.Value;
    
    public static bool operator >(StockQuantity left, int right) => left.Value > right;
    public static bool operator <(StockQuantity left, int right) => left.Value < right;
    public static bool operator >=(StockQuantity left, int right) => left.Value >= right;
    public static bool operator <=(StockQuantity left, int right) => left.Value <= right;
    
    // Arithmetic operators
    public static StockQuantity operator +(StockQuantity stock, int quantity) => stock.Add(quantity);
    public static StockQuantity operator -(StockQuantity stock, int quantity) => stock.Deduct(quantity);
}
namespace CopilotSample.Api.Domain.Values
{
    public class CategoryName
    {
        public string Value { get; }
        public CategoryName(string value)
        {
            Value = value;
        }
        public override string ToString() => Value;
    }
}

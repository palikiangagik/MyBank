namespace MyBank.Domain.ValueObjects
{
    public readonly record struct Money
    {
        public decimal Value { get; init; }
        public Money(decimal value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Money amount can't be negative.");
            Value = value;
        }
        
        public static implicit operator Money(decimal value) => new(value);
        public static implicit operator decimal(Money money) => money.Value;

        public static bool operator <(Money a, Money b) => a.Value < b.Value;
        public static bool operator >(Money a, Money b) => a.Value > b.Value;
        public static bool operator <=(Money a, Money b) => a.Value <= b.Value;
        public static bool operator >=(Money a, Money b) => a.Value >= b.Value;
        public static Money operator +(Money a, Money b) => new(a.Value + b.Value);
        public static Money operator -(Money a, Money b)
        {
            if (a.Value < b.Value)
                throw new InvalidOperationException("Result of money subtraction cannot be negative.");
            return new(a.Value - b.Value);
        }

    }
}

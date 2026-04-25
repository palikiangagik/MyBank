using System.Numerics;

namespace MyBank.Domain.ValueObjects
{
    public readonly record struct IntId
    { 
        public int Value { get; init; }
        public IntId(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Id must be positive.");
            Value = value;
        }
        public static implicit operator int(IntId intId) => intId.Value;
        public static implicit operator IntId(int value) => new(value);
    }

    public readonly record struct StringId
    {
        public string Value { get; init; }
        public StringId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Id cannot be empty.", nameof(value));
            Value = value;
        }
        public static implicit operator string(StringId stringId) => stringId.Value;
        public static implicit operator StringId(string value) => new(value);
    }
}

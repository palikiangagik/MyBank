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
}

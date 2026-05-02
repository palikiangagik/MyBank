namespace MyBank.Domain.ValueObjects
{
    public readonly record struct ClientName
    {
        public readonly string FirstName { get; init; }
        public readonly string LastName { get; init; }

        public ClientName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
            FirstName = firstName;
            LastName = lastName;
        }
    }
}

namespace MyBank.Api.DTO
{
    public record DestinationAccountDTO
    {
        public record ClientName
        {
            public required string FirstName { get; init; }
            public required string LastName { get; init; }
        }

        public required int Id { get; init; }
        public required string Code { get; init; }
        public required ClientName Name { get; init; }
    }
}

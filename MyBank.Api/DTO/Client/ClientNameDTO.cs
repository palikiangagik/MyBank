namespace MyBank.Api.DTO.Client
{
    public record ClientNameDTO
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
    }
}

using MyBank.Api.DTO.Client;

namespace MyBank.Api.DTO.Accounts
{
    public record DestinationAccountDTO
    {
        public required int Id { get; init; }
        public required string Code { get; init; }
        public required ClientNameDTO Name { get; init; }
    }
}

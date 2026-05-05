using MyBank.Application.DTO.Client;

namespace MyBank.Application.DTO.Transactions
{
    public record TransferPartyDTO
    {
        public required ClientNameDTO Name { get; init; }
        public required string AccountCode { get; init; }
    }
}

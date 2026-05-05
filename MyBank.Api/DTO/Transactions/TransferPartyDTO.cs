using MyBank.Api.DTO.Client;

namespace MyBank.Api.DTO.Transactions
{
    public class TransferPartyDTO
    {
        public required ClientNameDTO Name { get; init; }
        public required string AccountCode { get; init; }
    }
}

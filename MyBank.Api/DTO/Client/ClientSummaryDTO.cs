using MyBank.Api.DTO.Accounts;

namespace MyBank.Api.DTO.Client
{ 
    public record ClientSummaryDTO
    {
        public required int Id { get; init; }
        public required ClientNameDTO Name { get; init; }
        public required decimal TotalBalance { get; init; }
        public required AccountSummaryListDTO Accounts { get; init; }
    }
}

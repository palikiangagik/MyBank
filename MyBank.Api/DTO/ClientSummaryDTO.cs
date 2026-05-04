namespace MyBank.Api.DTO
{
    public record ClientSummaryAccountItemDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
 
    public record ClientSummaryDTO
    {
        public string ClientName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public List<ClientSummaryAccountItemDTO> Accounts { get; set; } = [];
    }
}

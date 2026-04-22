namespace MyBank.Web.Contracts.Account.DTO
{
    public class TransferMoneyDTO
    {
        public string SenderCode { get; set; }
        public string RecepientCode { get; set; }
        public string RecepientUserName { get; set; }
        public decimal Amount { get; set; }
    }
}

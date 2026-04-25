namespace MyBank.Application.DTO
{
    public record TransferTransactionDTO(DateTime CreatedAt, decimal Amount, 
        string SenderAccountCode, string ReceiverAccountCode);
}

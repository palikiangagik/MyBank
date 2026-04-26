namespace MyBank.Application.DTO
{
    public record WithdrawalTransactionDTO(DateTime CreatedAt, decimal Amount, string Code);
}

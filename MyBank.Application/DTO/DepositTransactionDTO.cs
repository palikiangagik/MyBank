namespace MyBank.Application.DTO
{
    public record DepositTransactionDTO(DateTime CreatedAt, decimal Amount, string Code);
}

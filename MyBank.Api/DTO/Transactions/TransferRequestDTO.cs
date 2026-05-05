using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTO.Transactions
{
    public record TransferRequestDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; init; }

        [Required]
        public int RecipientAccountId { get; init; }
    }
}

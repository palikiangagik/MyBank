using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTO
{
    public record WithdrawalRequestDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }
    }
}

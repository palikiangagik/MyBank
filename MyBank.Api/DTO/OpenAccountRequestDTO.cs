using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTO
{
    public record OpenAccountRequestDTO
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Amount can't be negative.")]
        public decimal Amount { get; set; } = 0;
    }
}

using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTO
{
    public record LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }
        [Required]
        public required string Password { get; init; }
        public bool RememberMe { get; init; }
    }
}

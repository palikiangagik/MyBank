namespace MyBank.Infrastructure.DTO.Auth
{
    public record LoginClientDTO
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required bool RememberMe { get; init; }
    }
}

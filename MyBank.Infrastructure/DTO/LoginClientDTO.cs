namespace MyBank.Infrastructure.DTO
{
    public record LoginClientDTO
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required bool RememberMe { get; init; }
    }
}

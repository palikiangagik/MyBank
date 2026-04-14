using MyBank.Portal.Infrastructure;

namespace MyBank.Portal.Contracts.Account
{
    // Predefined errors (avoids magic strings)
    public static class Errors
    {
        public static Error AccountNotFound { get; } = new("AccountNotFound", ErrorType.NotFound, "Specified account was not found.");
        public static Error NonPositiveAmount { get; } = new("NonPositiveAmount", ErrorType.Validation, "Amount must be greater than zero.");
    }
}

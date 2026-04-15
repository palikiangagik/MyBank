using MyBank.Portal.Infrastructure;

namespace MyBank.Portal.Contracts.Account
{
    // Predefined errors (avoids magic strings)
    public static class Errors
    {
        public static Error UserNotFound { get; } = new("UserNotFound", ErrorType.NotFound, "Specified user was not found.");
        public static Error AccountNotFound { get; } = new("AccountNotFound", ErrorType.NotFound, "Specified account was not found.");
        public static Error NonPositiveAmount { get; } = new("NonPositiveAmount", ErrorType.Validation, "Amount must be greater than zero");
        public static Error NegativeAmount { get; } = new("NegativeAmount", ErrorType.Validation, "Amount can't be less than zero.");
        public static Error InvalidPagingParameters { get; } = new("InvalidPagingParameters", ErrorType.Validation, "Page number and size must be greater than zero.");
        public static Error NotEnoughBalance { get; } = new Error("NotEnoughBalance", ErrorType.Validation, "Not enough balance to perform the operation.");
        public static Error SelfTransferNotAllowed { get; } = new("SelfTransferNotAllowed", ErrorType.Validation, "Transferring to the same account is not allowed.");
    }
}

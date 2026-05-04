using CorePrimitives;

namespace MyBank.Domain.Common
{
    public static class Errors
    {
        public static Error ClientNotFound { get; } = new Error(ErrorType.NotFound, "Client not found.");
        public static Error AccountNotFound { get; } = new Error(ErrorType.NotFound, "Account not found.");
        public static Error InsufficientFunds { get; } = new Error(ErrorType.Conflict, "Insufficient funds.");
        public static Error OperationOnClosedAccount { get; } = new Error(ErrorType.Conflict, "Can't perform this operation. Account is closed.");
        public static Error AccountAlreadyClosed { get; } = new Error(ErrorType.Conflict, "Can't close the account. Account is already closed.");
        public static Error CannotCloseAccountWithBalance { get; } = new Error(ErrorType.Conflict, "Can't close the account. Withdraw the funds first.");
    }
}

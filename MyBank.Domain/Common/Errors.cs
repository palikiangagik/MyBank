using CorePrimitives;

namespace MyBank.Domain.Common
{
    public static class Errors
    {
        public static Error ClientNotFound { get; } = new Error("Domain.ClientNotFound", "Client not found.");
        public static Error AccountNotFound { get; } = new Error("Domain.AccountNotFound", "Account not found.");
        public static Error InsufficientFunds { get; } = new Error("Domain.InsufficientFunds", "Insufficient funds.");
        public static Error OperationOnClosedAccount { get; } = new Error("Domain.OperationOnClosedAccount", "Can't perform this operation. Account is closed.");
        public static Error AccountAlreadyClosed { get; } = new Error("Domain.AccountAlreadyClosed", "Can't close the account. Account is already closed.");
        public static Error CannotCloseAccountWithBalance { get; } = new Error("Domain.CannotCloseAccountWithBalance", "Can't close the account. Withdraw the funds first.");
    }
}

using CorePrimitives;

namespace MyBank.Domain.Common
{
    public static class Failures
    {
        public static Failure InsufficientFunds { get; } = new Failure("InsufficientFunds", "Insufficient funds.");
        public static Failure OperationOnClosedAccount { get; } = new Failure("OperationOnClosedAccount", "Can't perform this operation. Account is closed.");
        public static Failure AccountAlreadyClosed { get; } = new Failure("AccountAlreadyClosed", "Can't close the account. Account is already closed.");
        public static Failure CannotCloseAccountWithBalance { get; } = new Failure("CannotCloseAccountWithBalance", "Can't close the account. Withdraw the funds first.");
    }
}

using MyBank.Portal.Infrastructure;
using System.Threading.Tasks;

namespace MyBank.Portal.Contracts.Account
{
    // TODO: add exceptions and error type descrptions to the comments
    public interface IAccountService
    {

        /// <summary>
        /// Get account list for the curuser. Only active accounts are returned.
        /// </summary>
        /// <param name="currentUserId">The ID of the current user.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>
        /// <see cref="Result{AccountListDTO}" /> If successfully retrieved.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If page or pageSize is less than 1.</exception>
        /// <exception cref="ArgumentNullException">If currentUserId is null or empty.</exception>
        Task<Result<AccountListDTO>> GetAccounts(string currentUserId, int page, int pageSize);


        /// <summary>
        /// Deposit the specified amount to the account.
        /// </summary>
        /// <param name="currentUserId">The ID of the current user.</param>
        /// <param name="accountId">The ID of the account to deposit into.</param>
        /// <param name="amount">The amount of deposit. Must be greater than zero.</param>
        ///  <returns>
        ///  <see cref = "Result.Success" /> If successfully deposited.
        ///  <see cref="Errors.NonPositiveAmount"/> If the amount is less than or equal to zero.
        ///  <see cref="Errors.AccountNotFound"/> If the account is not found or does not belong to the user.
        ///  </returns>
        ///  <exception cref="ArgumentNullException">If currentUserId is null or empty.</exception>
        Task<Result> Deposit(string currentUserId, int accountId, decimal amount);

    }
}

using MyBank.Portal.Infrastructure;
using System.Threading.Tasks;

namespace MyBank.Portal.Contracts.Account
{
    public interface IAccountService
    {
        /// <summary>
        /// Get account list for the current user. Only active accounts are returned.
        /// </summary>
        /// <param name="currentUserId"> The ID of the current user.</param>
        /// <param name="page"> The page number. Should be greater than zero.</param>
        /// <param name="pageSize"> The number of items per page. Should be greater than zero.</param>
        /// <returns>
        /// <see cref="Result{AccountListDTO}" /> If successfully retrieved.
        /// <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        /// <see cref="Errors.InvalidPagingParameters"/> If the paging parameters are invalid.
        /// </returns>
        Task<Result<AccountListDTO>> GetAccountsAsync(string currentUserId, int page, int pageSize);

        /// <summary>
        /// Get destination account list for the current user. 
        /// </summary>
        /// <param name="currentUserId"> The ID of the current user.</param>
        /// <param name="page"> The page number. Should be greater than zero.</param>
        /// <param name="pageSize"> The number of items per page. Should be greater than zero.</param>
        /// <returns>
        /// <see cref="Result{DestinationAccountListDTO}" /> If successfully retrieved.
        /// <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        /// <see cref="Errors.InvalidPagingParameters"/> If the paging parameters are invalid.
        /// </returns>
        Task<Result<DestinationAccountListDTO>> GetDestinationAccountsAsync(string currentUserId, int page, int pageSize);

        /// <summary>
        /// Create new account for the current user
        /// </summary>
        /// <param name="currentUserId"> The ID of the current user.</param>
        /// <param name="balance"> The initial balance of the account. Must be greater than or equal to zero.</param>
        /// <returns>
        /// <see cref = "Result{AccountOpenDTO}" /> If successfully opened.
        /// <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        /// <see cref="Errors.NegativeAmount"/> If the amount is less than zero.
        /// </returns>
        Task<Result<AccountOpenDTO>> OpenNewAccountAsync(string currentUserId, decimal balance);

        /// <summary>
        /// Close current user's account.
        /// </summary>
        /// <param name="currentUserId"> The ID of the current user.</param>
        /// <param name="accountId"> The ID of the account to close.</param>
        /// <returns>
        /// <see cref="Result.Success"/> If successfully closed.
        /// <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        /// <see cref="Errors.AccountNotFound"/> If the account is not found or does not belong to the user.
        /// </returns>
        Task<Result> CloseAccountAsync(string currentUserId, int accountId);

        /// <summary>
        /// Deposit the specified amount to the account.
        /// </summary>
        /// <param name="currentUserId"> The ID of the current user.</param>
        /// <param name="accountId"> The ID of the account to deposit into.</param>
        /// <param name="amount"> The amount of deposit. Must be greater than zero.</param>
        ///  <returns>
        ///  <see cref = "Result.Success" /> If successfully deposited.
        ///  <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        ///  <see cref="Errors.NonPositiveAmount"/> If the amount is less than or equal to zero.
        ///  <see cref="Errors.AccountNotFound"/> If the account is not found or does not belong to the user.
        ///  </returns>
        Task<Result> DepositAsync(string currentUserId, int accountId, decimal amount);


        /// <summary>
        /// Withdraw the specified amount from the account.
        /// </summary>
        /// <param name="currentUserId"> The ID of the current user.</param>
        /// <param name="accountId"> The ID of the account to withdraw from.</param>
        /// <param name="amount"> The amount to withdraw. Must be greater than zero.</param>
        /// <returns>
        /// <see cref="Result.Success"/> If successfully withdrawn.
        /// <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        /// <see cref="Errors.NonPositiveAmount"/> If the amount is less than or equal to zero.
        /// <see cref="Errors.AccountNotFound"/> If the account is not found or does not belong to the user.
        /// <see cref="Errors.NotEnoughBalance"/> If not enough balance.
        /// </returns>
        Task<Result> WithdrawAsync(string currentUserId, int accountId, decimal amount);


        /// <summary>
        /// Transfer money
        /// </summary>
        /// <param name="currentUserId"> The ID of the current user.</param>
        /// <param name="accountFromId"> The ID of the account to transfer money from.</param>
        /// <param name="accountToId"> The ID of the account to transfer money to.</param>
        /// <param name="amount"> The amount of money to transfer. Must be greater than zero.</param>
        /// <returns>
        /// <see cref="Result.Success"/> If successfully transferred.
        /// <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        /// <see cref="Errors.AccountNotFound"/> If the accountFrom or accountTo is not found or accountFrom does not belong to the user.
        /// <see cref="Errors.NonPositiveAmount"/> If the amount is less than or equal to zero.
        /// <see cref="Errors.NotEnoughBalance"/> If not enough balance. 
        /// <see cref="Errors.SelfTransferNotAllowed"/> If accountFromId and accountToId are the same.
        /// </returns>
        Task<Result> TransferMoneyAsync(string currentUserId, int accountFromId, int accountToId, decimal amount);

        /// <summary>
        /// Get transaction history for specified user.
        /// </summary>
        /// <param name = "currentUserId"> The ID of the current user.</param>
        /// <param name="page"> The page number. Should be greater than zero.</param>
        /// <param name="pageSize"> The number of items per page. Should be greater than zero.</param>
        /// <returns>
        /// <see cref="Result{TransactionListDTO}" /> If successfully retrieved.
        /// <see cref="Errors.UserNotFound"/> If currentUserId is not found.
        /// <see cref="Errors.InvalidPagingParameters"/> If the paging parameters are invalid.
        /// </returns>
        Task<Result<TransactionListDTO>> GetTransactionHistoryAsync(string currentUserId, int page, int pageSize);
    }
}

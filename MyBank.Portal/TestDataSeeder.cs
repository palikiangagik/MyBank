using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyBank.Portal.Contracts.Account;
using MyBank.Portal.Data;
using MyBank.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MyBank.Portal
{
    public class TestDataSeeder
    {
        readonly private IAccountService _accountService;
        readonly private UserManager<IdentityUser> _userManager;
        readonly private ILogger<TestDataSeeder> _logger;

        public TestDataSeeder(IAccountService accountService, 
            UserManager<IdentityUser> userManager, ILogger<TestDataSeeder> logger)
        {
            _accountService = accountService;
            _userManager = userManager;
            _logger = logger;
        }        
        
        public async Task Run()
        {
            try {
                // Check if users already exist to avoid seeding multiple times
                if (await _userManager.Users.AnyAsync())
                    return;

                await Seed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred seeding the DB.");
                throw;
            }
        }

        private async Task Seed()
        {

            Dictionary<string, int> userIdAccountIdMap = new Dictionary<string, int>();
            Dictionary<string, string> emailUserIdMap = new Dictionary<string, string>();

            // filling with users with random start balance
            var seedData = new List<(string user, decimal balance)>
            {
                ( "andrew@mail.com", 300000m ),
                ( "james@mail.com", 13000m ),
                ( "john@mail.com", 20000m ),
                ( "olivia@mail.com", 7500m ),
                ( "sophia@mail.com", 3250m )
            };

            foreach (var (email, balance) in seedData)
            {
                string userId = await CreateUser(email);
                int accountId = await CreateAccount(userId, balance);
                userIdAccountIdMap[userId] = accountId;
                emailUserIdMap[email] = userId;
            }

            // creating multiple transfers from andrew to james to fill the transaction history
            string andrewUserId = emailUserIdMap["andrew@mail.com"];
            int andrewAccountId = userIdAccountIdMap[andrewUserId];
            int jamesAccountId = userIdAccountIdMap[emailUserIdMap["james@mail.com"]];
            for (decimal amount = 10; amount < 400; amount += 10)
            {
                await CreateTransferTransaction(andrewUserId, andrewAccountId, jamesAccountId, amount);
            }
        }

        private async Task<string> CreateUser(string email)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, "1qaz@WSX");

            if (!result.Succeeded)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Identity Error: {errorMessages}");
            }

            return user.Id;
        }

        private async Task<int> CreateAccount(string userId, decimal balance)
        {
            var result = await _accountService.OpenNewAccountAsync(userId, balance);
            if (!result.IsSuccess)
            {
                throw new Exception($"Failed to create account for user with id {userId}: {result.Error}");
            }
            return result.Value.Id;
        }

        private async Task CreateTransferTransaction(string userFromId, 
            int accountFromId, int accountToId, decimal amount)
        {
            var result = await _accountService.TransferMoneyAsync(userFromId, accountFromId, 
                accountToId, amount);
            if (!result.IsSuccess)
            {
                throw new Exception($"Failed to create transfer transaction from account with id {accountFromId} to account " +
                    $"with id {accountToId}: {result.Error}");
            }
        }

        
    }
}

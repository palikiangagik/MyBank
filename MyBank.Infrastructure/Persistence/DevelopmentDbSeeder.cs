using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.UseCases;

namespace MyBank.Infrastructure.Persistence
{
    public class DevelopmentDbSeeder
    {
        readonly private UserManager<IdentityUser> _userManager;
        readonly private AccountsUseCases _accountUseCases;

        public DevelopmentDbSeeder(UserManager<IdentityUser> userManager, 
            AccountsUseCases accountUseCases)
        {
            _accountUseCases = accountUseCases;
            _userManager = userManager;
        }

        public async Task Run()
        {
            // Check if users already exist to avoid seeding multiple times
            if (await _userManager.Users.AnyAsync())
                return;

            await Seed();
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
            var result = await _accountUseCases.OpenAccountAsync(userId, balance);
            if (!result.IsSuccess)
                throw new Exception($"Failed to create account for user with id {userId}: {result.Failure}");
            return result.Value.Id;
        }

        private async Task CreateTransferTransaction(string userFromId,
            int accountFromId, int accountToId, decimal amount)
        {
            var result = await _accountUseCases.TransferAsync(userFromId, accountFromId, accountToId, amount);
            if (!result.IsSuccess)
            {
                throw new Exception($"Failed to create transfer transaction of {amount} from account with " +
                    $"id {accountFromId} to account " +
                    $"with id {accountToId}: {result.Failure}");
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.UseCases;
using MyBank.Infrastructure.Identity;

namespace MyBank.Infrastructure.Persistence
{
    public class DevelopmentDbSeeder
    {
        readonly private ClientIdentityService _clientIdentityService;
        readonly private UserManager<IdentityUser> _userManager;
        readonly private AccountsUseCases _accountUseCases;

        public DevelopmentDbSeeder(
            ClientIdentityService clientIdentityService,
            UserManager<IdentityUser> userManager, 
            AccountsUseCases accountUseCases)
        {
            _clientIdentityService = clientIdentityService;
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

            Dictionary<int, int> clientIdAccountIdMap = new();
            Dictionary<string, int> emailClientIdMap = new();

            // filling with users with random start balance
            var seedData = new List<(string firstName, string lastName, string email, decimal balance)>
            {
                ( "Andrew", "Smith", "andrew@mail.com", 300000m ),
                ( "James", "Johnson", "james@mail.com", 13000m ),
                ( "John", "Williams", "john@mail.com", 20000m ),
                ( "Olivia", "Brown", "olivia@mail.com", 7500m ),
                ( "Sophia", "Garcia", "sophia@mail.com", 3250m )
            };

            foreach (var (firstName, lastName, email, balance) in seedData)
            {
                int clientId = await CreateUser(firstName, lastName, email);
                int accountId = await CreateAccount(clientId, balance);
                clientIdAccountIdMap[clientId] = accountId;
                emailClientIdMap[email] = clientId;
            }

            // creating multiple transfers from andrew to james to fill the transaction history
            int andrewClientId = emailClientIdMap["andrew@mail.com"];
            int andrewAccountId = clientIdAccountIdMap[andrewClientId];
            int jamesAccountId = clientIdAccountIdMap[emailClientIdMap["james@mail.com"]];
            for (decimal amount = 10; amount < 400; amount += 10)
            {
                await CreateTransferTransaction(andrewClientId, andrewAccountId, jamesAccountId, amount);
            }
        }

        private async Task<int> CreateUser(string firstName, string lastName, string email)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _clientIdentityService.RegisterClientAsync(new DTO.RegisterClientDTO
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = "1qaz@WSX"
            });

            if (!result.Succeeded)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Identity Error: {errorMessages}");
            }

            return result.Value.ClientId;
        }

        private async Task<int> CreateAccount(int clientId, decimal balance)
        {
            var result = await _accountUseCases.OpenAccountAsync(clientId, balance);
            if (result.Failed)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create account for user with id {clientId}: {errorMessages}");
            }
            return result.Value.Id;
        }

        private async Task CreateTransferTransaction(int clientFromId,
            int accountFromId, int accountToId, decimal amount)
        {
            var result = await _accountUseCases.TransferAsync(clientFromId, accountFromId, accountToId, amount);
            if (result.Failed)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create transfer transaction of {amount} from account with " +
                    $"id {accountFromId} to account " +
                    $"with id {accountToId}: {errorMessages}");
            }
        }
    }
}

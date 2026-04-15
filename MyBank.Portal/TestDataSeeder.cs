using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        readonly private MyBankPortalContext _context;
        readonly private UserManager<IdentityUser> _userManager;
        readonly private ILogger<Program> _logger;

        public TestDataSeeder(MyBankPortalContext context, 
            UserManager<IdentityUser> userManager, ILogger<Program> logger)
        {
            _context = context;
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
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred seeding the DB.");
                throw;
            }
        }

        private async Task Seed()
        {
            var seedData = new List<(string user, decimal balance)>
            {
                ( "andrew@mail.com", 3000m ),
                ( "james@mail.com", 13000m ),
                ( "john@mail.com", 20000m ),
                ( "olivia@mail.com", 7500m ),
                ( "sophia@mail.com", 3250m )
            };

            foreach (var (email, balance) in seedData)
            {
                var user = await CreateUser(email);
                await CreateAccount(user, balance);
            }
        }

        private async Task<IdentityUser> CreateUser(string email)
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

            return user;
        }

        private async Task CreateAccount(IdentityUser user, decimal balance)
        {
            await _context.Accounts.AddAsync(new Account { 
                User = user, 
                Balance = balance
            });
        }
    }
}

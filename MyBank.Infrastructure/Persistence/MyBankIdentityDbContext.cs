using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MyBank.Infrastructure.Persistence
{
    public class MyBankIdentityDbContext : IdentityDbContext<IdentityUser>
    { 
        public MyBankIdentityDbContext(DbContextOptions<MyBankIdentityDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

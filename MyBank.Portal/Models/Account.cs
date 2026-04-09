using Microsoft.AspNetCore.Identity;

namespace MyBank.Portal.Models
{   public class Account
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public decimal Balance { get; set; }
    }
}

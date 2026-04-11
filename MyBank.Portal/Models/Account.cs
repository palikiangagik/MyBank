using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyBank.Portal.Models
{   
    public class Account
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public decimal Balance { get; set; }

        // To avoid concurrency issues
        [Timestamp] 
        public byte[] RowVersion { get; set; }
    }
}

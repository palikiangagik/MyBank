using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyBank.Portal.Data.Models
{   
    public class Account
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public decimal Balance { get; set; }
        [Required]
        public bool IsClosed { get; set; } = false;

        // To avoid concurrency issues
        [Timestamp] 
        public byte[] RowVersion { get; set; }
    }
}

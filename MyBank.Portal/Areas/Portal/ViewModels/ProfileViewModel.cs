using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBank.Portal.Areas.Portal.ViewModels
{
    public class ProfileAccountViewItem
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
    }

    public class ProfileViewModel
    {
        
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public List<ProfileAccountViewItem> Accounts { get; set; }
    }
}

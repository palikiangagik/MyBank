using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBank.Web.Areas.Web.ViewModels
{
    public class ClientAccountViewItem : BaseViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Balance { get; set; }
    }

    public class ClientViewModel
    {
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public List<ClientAccountViewItem> Accounts { get; set; } = [];
    }
}

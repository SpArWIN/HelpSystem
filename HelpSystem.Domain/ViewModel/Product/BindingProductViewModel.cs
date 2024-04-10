using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class BindingProductViewModel
    {
        public  int ? Id { get; set; }
        public string NameProduct { get; set; }
        public string InventoryCod { get; set; }
       public int TotalCount { get; set; }
    }
}

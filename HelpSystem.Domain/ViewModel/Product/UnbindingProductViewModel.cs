using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class UnbindingProductViewModel
    {
        public Guid ProfileId { get; set; }
        public string NameProduct { get; set; }
        public string Code { get; set; }
        public int CountUnbinding { get; set; }
    }
}

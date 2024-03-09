using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Invoice
{
    public class InvoiceShowViewModel
    {
        public Guid Id { get; set; }
        public string NumberInvoice { get; set; }
        public string DateCreated { get; set; }
    }
}

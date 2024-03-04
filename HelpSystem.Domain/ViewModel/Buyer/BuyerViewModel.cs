using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Buyer
{
    public class BuyerViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Введите имя покупателя")]
        public string BuyerName { get; set; } = string.Empty;
    }
}

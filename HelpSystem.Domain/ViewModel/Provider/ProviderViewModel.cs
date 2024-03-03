using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Provider
{
    public class ProviderViewModel
    {
        [Required(ErrorMessage = "Укажите наименование поставщика")]
        public string ProviderName { get; set; }
    }
}

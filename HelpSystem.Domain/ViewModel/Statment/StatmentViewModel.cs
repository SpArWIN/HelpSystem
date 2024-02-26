using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Enum;

namespace HelpSystem.Domain.ViewModel.Statment
{
    public class StatmentViewModel
    {
     
      public DateTime DataCreated { get; set; }

      public StatusStatement Status { get; set; }
        [Required(ErrorMessage = "Укажите причину создания")]
      public string Reason { get; set; }
        [Required(ErrorMessage = "Подробно опишите проблему")]
        [MinLength(20,ErrorMessage = "Количество символов должно быть больше 20")]
         public string Description { get; set; }

    }
}

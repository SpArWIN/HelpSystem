using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Statment
{
    public class StatmentViewModel
    {

        public string? DataCreated { get; set; }

        public string? Status { get; set; }
        [Required(ErrorMessage = "Укажите причину создания")]
        public string Reason { get; set; }
        [Required(ErrorMessage = "Подробно опишите проблему")]
        [MinLength(20, ErrorMessage = "Количество символов должно быть больше 20")]
        public string Description { get; set; }



    }
}

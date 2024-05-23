using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Account
{
    public class RecoveryProfile
    {
        //Класс для восстановления пароля
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [MinLength(8,ErrorMessage ="Минимальная длина пароля должна составлять 8 символов")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string NewDubletPassword { get; set; }
    }
}

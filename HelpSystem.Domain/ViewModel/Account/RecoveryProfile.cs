using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Account
{
    public class RecoveryProfile
    {
        //Класс для восстановления пароля
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Введите новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string NewDubletPassword { get; set; }
    }
}

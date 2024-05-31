using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Account
{
    public class RegisterVIewModel
    {
        [Required(ErrorMessage = "Укажите имя")]
        [MaxLength(20, ErrorMessage = "Имя должно иметь длину меньше 20 символов")]
        [MinLength(3, ErrorMessage = "Имя должно иметь длину больше 3 символов")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Укажите логин")]
        [MaxLength(20, ErrorMessage = "Логин должно иметь длину меньше 20 символов")]
        [MinLength(6, ErrorMessage = "Логин должен иметь длину больше 6 символов")]
        public string Login { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль")]
        [MinLength(8, ErrorMessage = "Пароль должен иметь длину не меньше 8 символов")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
        [EmailAddress(ErrorMessage = "Укажите корректную почту")]
        [Required(ErrorMessage = "Укажите почту")]
        public string Email { get; set; }

    }
}

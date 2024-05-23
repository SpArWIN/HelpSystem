using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Product.Role;
using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Profile
{
    public class ProfileViewModel
    {
        public Guid Id { get; set; }

    
        [MaxLength(20, ErrorMessage = "Фамилия должна иметь длину меньше 20 символов")]
        [MinLength(6, ErrorMessage = "Фамилия должно иметь длину больше 6 символов")]
        public string? Surname { get; set; }
        //Отчество
        [MaxLength(20, ErrorMessage = "Отчество должно иметь длину меньше 20 символов")]
        public string? LastName { get; set; }
        public string? Description { get; set; }
        [MaxLength(20, ErrorMessage = "Имя должно иметь длину меньше 20 символов")]
        [MinLength(6, ErrorMessage = "Имя должно иметь длину больше 6 символов")]
        public string? Name { get; set; }
        public List<BindingProductViewModel>? UserPdocut { get; set; }
        //Тут возьмем подсчёт общего количества товаров, закреплённых за юзверем
        public int SumTotalProducts { get; set; }

        //Роль пользоавателя 
        public string? RoleName { get; set; }
        //id Роли
        public int RoleId { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        public string? Email { get; set; }
       
        //Для роли пользователя 
        public List<RoleViewModel>? Roles { get; set; }
    }

}

using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Product.Role;
using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Profile
{
    public class ProfileViewModel
    {
        public Guid Id { get; set; }

        public byte? Age { get; set; }
        public string? Surname { get; set; }
        //Отчество
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public List<BindingProductViewModel>? UserPdocut { get; set; }
        //Тут возьмем подсчёт общего количества товаров, закреплённых за юзверем
        public int SumTotalProducts { get; set; }

        //Роль пользоавателя 
        public string? RoleName { get; set; }
        //id Роли
        public int RoleId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Age.HasValue && Age < 0)
            {
                yield return new ValidationResult("Возраст не может быть отрицательным.", new[] { nameof(Age) });
            }
        }
        //Для роли пользователя 
        public List<RoleViewModel>? Roles { get; set; }
    }

}

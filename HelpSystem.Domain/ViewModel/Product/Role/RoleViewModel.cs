using HelpSystem.Domain.Enum;

namespace HelpSystem.Domain.ViewModel.Product.Role
{
    //Класс для отображения роли
    public class RoleViewModel
    {
        public int Id { get; set; }
        public UserRoleType RoleType { get; set; }
        public string? RoleName { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.Enum
{
    public enum UserRoleType
    {
        [Display(Name = "Пользователь")]
        User = 1,
        [Display(Name = "Администратор")]
        Admin = 2,

    }
}

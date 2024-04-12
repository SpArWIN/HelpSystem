using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Enum;

namespace HelpSystem.Domain.ViewModel.Product.Role
{
    //Класс для отображения роли
    public class RoleViewModel
    {
        public int Id { get; set; }
        public UserRoleType RoleType { get; set; }
        public string?  RoleName {get; set; }

}
}

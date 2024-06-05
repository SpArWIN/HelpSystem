using HelpSystem.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.Entity
{
    public class Role
        
    {
        
        [Key]  public int Id { get; set; }
        public UserRoleType RoleType { get; set; }
        public List<User> Users { get; set; }
    }
}

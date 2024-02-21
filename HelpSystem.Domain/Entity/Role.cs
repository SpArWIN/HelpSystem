using HelpSystem.Domain.Enum;

namespace HelpSystem.Domain.Entity
{
    public class Role
    {
        public int Id { get; set; }
        public UserRoleType RoleType { get; set; }
        public List<User> Users { get; set; }
    }
}

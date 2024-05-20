using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpSystem.Domain.Entity
{
    [Table("Users")]
    public class User
    {

        public Guid Id { get; set; }
        [MaxLength(40)]
        public string Name { get; set; }
        [MaxLength(30)]

        public string Login { get; set; }

        public string Password { get; set; }
        public int RoleId { get; set; }
        public virtual Role Roles { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual List<Statement>? Statement { get; set; }

    }
}

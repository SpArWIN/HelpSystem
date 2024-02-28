﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HelpSystem.Domain.Entity
{
    [Table("Profiles")]
    public class Profile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Surname { get; set; }
        //Отчество
        public string? LastName { get; set; }
        public byte? Age { get; set; }

    }
}

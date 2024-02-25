﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Profile
{
    public class ProfileViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Укажите возраст")]
        [Range(0, 150, ErrorMessage = "Диапазон возраста должен быть от 0 до 150")]
        public byte ? Age { get; set; }
        public string?  Surname { get; set; }
        //Отчество
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Age.HasValue && Age < 0)
            {
                yield return new ValidationResult("Возраст не может быть отрицательным.", new[] { nameof(Age) });
            }
        }
    }

}

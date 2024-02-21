using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.Enum
{
    public enum StatusStatement
    {
        [Display(Name = "На рассмотрении")]
        UnderConsideration = 0,
        [Display(Name = "Рассмотрена")]
        Reviewed = 1,
        [Display(Name = "Завершена")]
        Completed = 2,

    }
}

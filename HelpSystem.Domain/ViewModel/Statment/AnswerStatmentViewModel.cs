using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Statment
{
    public class AnswerStatmentViewModel
    {
        public Guid? Id { get; set; }
        public string? Reason { get; set; }
        public string? Description { get; set; }
        // Ответ на заявку
        public string? Response { get; set; }
        public string? FullName { get; set; }
    }
}

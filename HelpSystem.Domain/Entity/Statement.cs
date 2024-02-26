using HelpSystem.Domain.Enum;

namespace HelpSystem.Domain.Entity
{
    //Заявка пользователя
    public class Statement
    {
        public Guid ID { get; set; }
        public DateTime DataCreated { get; set; }
        public string Reason { get; set; }
        public DateTime ? DateCompleted { get; set; }
        public StatusStatement Status { get; set; }
        public string Comments { get; set; }
        public virtual User? User { get; set; }

    }
}

namespace HelpSystem.Domain.ViewModel.Statment
{
    public class GetAllStatments
    {
        public string DataCreated { get; set; }
        public string? Status { get; set; }
        //Фамилия
        public string? LastName { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public string? FullName
        {
            get { return $"{LastName} {Name} {Patronymic}"; }
            set { }
        }



        public string? Reason { get; set; }
        public string? Description { get; set; }
        // Ответ на заявку
        public string? Response { get; set; }

    }
}

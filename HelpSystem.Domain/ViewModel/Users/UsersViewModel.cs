namespace HelpSystem.Domain.ViewModel.Users
{
    public class UsersViewModel
    {
        public string Login { get; set; }
        public string? Surname { get; set; } //Фамилия
        //Отчество
        public string? LastName { get; set; }
        public string? Name { get; set; }
        //Роль
        public string Roles { get; set; }

        public byte? Age { get; set; }

    }
}

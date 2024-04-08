using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.ViewModel.Product;


namespace HelpSystem.Domain.ViewModel.Users
{
    public class UserInfoViewModel
    {
        //Модель для пользователя, собирательная инфа
        public string Login { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? SurName { get; set; }
        public string? Message { get; set; } // Сообщение о статусе прикрепленных товаров
        public string? FullName
        {
            get => $"{LastName} {Name} {SurName}";
            set { }
        }
        public List<BindingProductViewModel> ? UserProducts { get; set; }

        public int TotalProducts { get; set; }
    }
}

namespace HelpSystem.Domain.Entity
{
    public class Warehouse
    {
        public Warehouse()
        {
            Products = new List<Products>();
        }
        //TODO Добавишь поля
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual List<Products>? Products { get; set; }
        //Заморожен ли склад
        public bool IsFreeZing { get; set; }

        public bool  IsService { get; set; } = false; //  Создаем один склад, которыый будет для утилизации.



    }
}

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
        public bool IsFreeZing { get; set; } 

    }
}

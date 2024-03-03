namespace HelpSystem.Domain.Entity
{
    public class Warehouse
    {

        //TODO Добавишь поля
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual List<Products>? Products { get; set; }
    }
}

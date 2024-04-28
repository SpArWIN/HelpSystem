namespace HelpSystem.Domain.Entity
{
    public class Provider
    {
        public Provider()
        {
            Products = new List<Products>();
        }
        //TODO Добавишь поля,Поставщик, остальные данные добавишь по необходимости
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual List<Products>? Products { get; set; }
        public bool IsFreeZing { get; set; }

    }
}

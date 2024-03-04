namespace HelpSystem.Domain.Entity
{
    public class Buyer
    {
        //TODO Будет нужно, добавишь покупателей
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual List<Products>?  Products { get; set; }
    }
}

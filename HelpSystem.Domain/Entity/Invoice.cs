using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.Entity
{
    public class Invoice
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Укажите номер документа")]
        public string NumberDocument { get; set; } //Номер документа
        public DateTime CreationDate { get; set; }
        //Навигационное свойство на накладную
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}

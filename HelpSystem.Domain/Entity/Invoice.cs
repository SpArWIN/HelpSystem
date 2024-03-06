using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.Entity
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public  string NumberDocument { get; set; } //Номер документа
        public DateTime CreationDate { get; set; }
        //Навигационное свойство на накладную
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}

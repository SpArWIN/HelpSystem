using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Transfer
{
    public class TransferViewModel
    {
        public int Id { get; set; } //Id товара

       //public Guid ProductId { get; set; }
       public string NameProduct { get; set; } = string.Empty;
       public string CodeProduct { get; set; } = string.Empty;

       public Guid SourceWarehouseId { get; set; } // ID исходного склада
       public Guid DestinationWarehouseId { get; set; } //На какой склад перемещаем
       //Какое количество товара перемещаем
       public int CountTransfer { get; set; }
    }
}

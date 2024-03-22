using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    //Для того, чтобы выводить отдельно список товаров по штучно на склад
    // и отдельно список складов, делаем так
    //добавляя сюда поля для товара, а потом все это основывается на TransferProductViewModel
    public class TransferView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.ViewModel.Transfer;
using HelpSystem.Domain.ViewModel.Users;

namespace HelpSystem.Domain.ViewModel.Product.ProductAllInfo
{
    //Класс будет использоваться для собирательной всей информации о товаре, абсолютно
    public class MainProductInfo
    {
        // тут будет пользователь
        public string NameProduct { get; set; }
        public string InventoryCode { get; set; }

        public string Comments { get; set; }
        public string OriginalWarehouse { get; set; } // Изначальное место положение
        public string CurrentWarehouseName {get; set; } //Текущее местоположение

        public UsersViewModel?  Usver { get; set; } // По товару определим пользователя и запишем
        public List<TransferFindInfo>?  AllTransfersProducts { get; set; } //Все перемещения товара 
        public string DateInvouce { get; set; } // Дата поступления товара по накладной 
        public string NumberDocument { get; set;  } //Номер документа по которому поступил товар

    }
}

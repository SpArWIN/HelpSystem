using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Transfer
{
    //Класс будет использоватся для собирательной информации о всех перемещениях товара
    // как со склада, так и на склад
    public class TransferFindInfo
    {
        public string DateTimeIncoming { get; set; } //Дата перемещения товара на склад
        public string DateTimeOutgoing { get; set; } //Дата перемещения товара со склада
        public string SourceWarehouseName { get; set; } //Склад на которой пришло 
        public string DestinationWarehouseName { get; set; } //Склад с которого ушёл
        public string ?Comments  { get; set; } //Комментарий при перемещении товара

    }
}

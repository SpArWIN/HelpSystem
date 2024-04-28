namespace HelpSystem.Domain.ViewModel.Product
{
    public class BindingProductWarehouse
    {
        //Добавим сюда Id склада, чтобы определять, что товар переносится именно с этого склада
        // тк он может быть перенесён
        public Guid? WarehouseId { get; set; }
        public Guid UserId { get; set; }
        //public string ProductName { get; set; }
        //public string InventoryCode { get; set; }
        //Тут мы передаем количество товара, которо нужно привязать
        /*
         * И соответственно логика обработки будет, если указано недопустимое количество,
         * то будет возврат
         */
        //public int CountBinding { get; set; }


        public int ProductId { get; set; }


    }
}

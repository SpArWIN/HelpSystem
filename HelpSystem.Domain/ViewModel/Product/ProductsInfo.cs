namespace HelpSystem.Domain.ViewModel.Product
{
    //Класс для хранения инфы о товаре в отчёте
    public class ProductsInfo
    {
        public string ProductName { get; set; }
        public string InventoryCode { get; set; }
        //Количество на складе
        public int QuantityOnWarehouse { get; set; }
        //Доступно
        public int AvailableQuantity { get; set; }
        ////Перемещено
        //public int MovedQuantity { get; set; }
    }
}

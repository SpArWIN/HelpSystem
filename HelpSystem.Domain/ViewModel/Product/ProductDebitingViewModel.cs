namespace HelpSystem.Domain.ViewModel.Product
{
    //Для модели товара, как матрёшка
    public class ProductDebitingViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Inventory { get; set; }
        public string DataEntrance { get; set; } //дата поступления товара
        public string DateDebiting { get; set; } //Дата списания
        public string CommentsDebiting { get; set; }  //Комментарий по списанному товару

        public string OriginalWarehouse { get; set; } // Склад, на который товар изначально поступал.
        public string? DebitingWarehouse { get; set; } //А это склад, 
    }
}

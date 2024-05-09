namespace HelpSystem.Domain.ViewModel.Transfer
{
    public class TransferViewModel
    {
        public int Id { get; set; } //Id товара

    
        public Guid SourceWarehouseId { get; set; } // ID исходного склада
        public Guid DestinationWarehouseId { get; set; } //На какой склад перемещаем
                                                         //Какое количество товара перемещаем
        public int CountTransfer { get; set; }
        public string? Comments { get; set; } // Это коммментарий при перемещении, он также может идти и под списание- при массовом перемещении

        public string ? CommentDebiting { get; set; } // Комментарий при одиночном перемещении - под склад списания

    }
}

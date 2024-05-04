namespace HelpSystem.Domain.ViewModel.Transfer
{
    //Будет использоваться для окна с перемещением, точнее для отображения товара 
    public class TransferProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public IEnumerable<Entity.Warehouse>? Warehouses { get; set; }
        public Guid? DestinationWarehouseId { get; set; }     // Склад, на который был перемещен товар
    }
}

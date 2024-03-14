namespace HelpSystem.Domain.Entity
{
    public class Products
    {
        public Guid Id { get; set; }
        public string InventoryCode { get; set; } = string.Empty; //Инвертарный код
        //public string Type {get;set;} -если будет нужно, то добавим товару тип
        public string NameProduct { get; set; } = string.Empty;
        public string? Comments { get; set; }
        //Внешний ключ поставщика
        //public Guid ProviderId { get; set; }
        public virtual Provider? Provider { get; set; } //Поставщик
        ////Внешний ключ Склада
        //public Guid WarehouseID { get; set; }
        public virtual Warehouse?  Warehouse { get; set; } //На какой склад закидываем
        // Внешний ключ
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; } //К какому пользователю привязываем в случае чего


    }
}

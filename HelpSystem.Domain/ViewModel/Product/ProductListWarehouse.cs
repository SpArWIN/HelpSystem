﻿namespace HelpSystem.Domain.ViewModel.Product
{
    //Для списка товаров на складе 
    public class ProductListWarehouse
    {
        public int Id { get; set; }
        public string NameProduct { get; set; }
        public string CodeProduct { get; set; }

        public int AvailableCount { get; set; }
    }
}

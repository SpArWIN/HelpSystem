namespace HelpSystem.Domain.ViewModel.Product
{
    public class ProductShowViewModel
    {
        public string NumberDoc { get; set; } 
        public string NameProduct { get; set; }
        public string CodeProduct { get; set; }
        public string Warehouse { get; set; }
        public string Provider { get; set; }

        public int TotalCount { get; set; }
    }
}

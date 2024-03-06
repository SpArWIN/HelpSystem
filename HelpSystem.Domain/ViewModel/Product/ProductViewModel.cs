using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class ProductViewModel
    {
        public string NumberDocument { get; set; } = string.Empty; 
        public string InventoryCode { get; set; } = string.Empty; 
       
        public string NameProduct { get; set; } = string.Empty;
        public string? Comments { get; set; }
        [Required(ErrorMessage = "Выберите поставщика")]
        public string Provider { get; set; }
        [Required(ErrorMessage = "Выберите склад")]
        public string Warehouse { get; set; }
        //Прописываю количество позиций, для первой итерации прохождения по строкам
        [Required(ErrorMessage = "Укажите количество позиций")]
        [Range(1,1000,ErrorMessage = "Количество позиций не может быть меньше 1 или больше 1000")]
        public int CountPositon { get; set; }
        

    }
}

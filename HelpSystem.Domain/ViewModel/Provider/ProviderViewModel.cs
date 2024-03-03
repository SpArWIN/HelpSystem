using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Provider
{
    public class ProviderViewModel
    {
        public Guid? ProviderId { get; set; }
        [Required(ErrorMessage = "Укажите наименование поставщика")]
        public string ProviderName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace JewelryStore.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Неверный формат Email")]
        public string Email { get; set; }
    }
}

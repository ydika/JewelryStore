using System.ComponentModel.DataAnnotations;

namespace JewelryStore.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите Ваше имя")]
        [RegularExpression(@"[A-zА-я]*", ErrorMessage = "В имени должны быть только буквы")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите Вашу фамилию")]
        [RegularExpression(@"[A-zА-я]*", ErrorMessage = "В фамилии должны быть только буквы")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Неверный формат Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Не указан пароль")]
        [RegularExpression(@"(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{6,}",
            ErrorMessage = "Пароль должен быть не менее 6 символов в длину, содержать строчные и прописные латинские буквы, цифры и спец. символы.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class ChangeAccountDataViewModel
    {
        [Required(ErrorMessage = "Введите Ваше имя")]
        [RegularExpression(@"[A-zА-я]*", ErrorMessage = "В имени должны быть только буквы")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите Вашу фамилию")]
        [RegularExpression(@"[A-zА-я]*", ErrorMessage = "В фамилии должны быть только буквы")]
        public string SecondName { get; set; }

        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{6,}",
            ErrorMessage = "Пароль должен быть не менее 6 символов в длину, содержать строчные и прописные латинские буквы, цифры и спец. символы.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string PasswordConfirm { get; set; }
    }
}

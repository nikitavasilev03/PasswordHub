using System.ComponentModel.DataAnnotations;

namespace PasswordHub.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Введите E-mail")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "E-mail адрес не корректен")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}

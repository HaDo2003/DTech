using System.ComponentModel.DataAnnotations;

namespace DTech.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        [Display(Name = "Account")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

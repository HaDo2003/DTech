using System.ComponentModel.DataAnnotations;

namespace DTech.Models.ViewModel
{
    public class PasswordViewModel
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }


        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTech.Models.ViewModel
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Please enter your full name")]
        [Display(Name = "Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your name")]
        [Display(Name = "Account")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the confirm password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your date of birth")]
        [Display(Name = "Date of Birth")]
        public DateOnly DateOfBirth { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public string? Address { get; set; }
    }
}

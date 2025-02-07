using System.ComponentModel.DataAnnotations;

namespace Passport.Models.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name can't be blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string? ConfirmPassword { get; set; }
    }
}

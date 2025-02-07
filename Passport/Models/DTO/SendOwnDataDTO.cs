using System.ComponentModel.DataAnnotations;

namespace Passport.Models.DTO
{
    public class SendOwnDataDTO
    {
        [Required]
        [StringLength(20,MinimumLength = 2, ErrorMessage = "Name length can't be more than 20 symbols and 2 minimal.")]
        [RegularExpression(@"[^\d]+$", ErrorMessage = "First name cannot contain numbers.")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Second name length can't be more than 20 symbols and 2 minimal.")]
        [RegularExpression(@"[^\d]+$", ErrorMessage = "Second name cannot contain numbers.")]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"[^\d]+$", ErrorMessage = "Nationality cannot contain numbers.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Nationality length can't be more than 20 symbols and 2 minimal.")]
        public string? Nationality { get; set; }

        [Required]
        [RegularExpression(@"^(Woman|Man|woman|man|чоловік|чоловіча|жінка|жіноча)$", ErrorMessage = "Gender must be either 'Woman' or 'Man'.")]
        public string? Gender { get; set; }

        [Required]
        [RegularExpression(@"[^\d]+$", ErrorMessage = "City cannot contain numbers.")]
        public string? CityOfResidence { get; set; }

        [Required]
        public string? PhotoUrl { get; set; } 
    }
}

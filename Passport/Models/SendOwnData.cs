using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Passport.Models
{
    public class SendOwnData
    {
        [Key]
        public int Id { get; set; }  
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }  
        public DateTime DateOfBirth { get; set; } 
        public string? Nationality { get; set; }  
        public string? Gender { get; set; }  
        public string? CityOfResidence { get; set; }  
        public string? PhotoUrl { get; set; }  
        public string? Status { get; set; } = "Pending";  
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;  
        [JsonIgnore]
        public ICollection<PassportOrigin>? Passports { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public int? UserId { get; set; } 
    }
}

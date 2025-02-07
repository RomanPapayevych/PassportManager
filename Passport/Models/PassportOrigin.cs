using System.ComponentModel.DataAnnotations.Schema;

namespace Passport.Models
{
    public class PassportOrigin
    {
        public int Id { get; set; } 
        public string? PassportNumber { get; set; }  
        public string? IdentificationCode { get; set; }  
        public DateTime IssueDate { get; set; } 
        public DateTime ExpiryDate { get; set; } 
        [ForeignKey("SendOwnDataId")]
        public SendOwnData? sendOwnData { get; set; } 
        public int SendOwnDataId { get; set; } 
    }
}

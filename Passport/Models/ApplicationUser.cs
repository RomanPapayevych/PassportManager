    using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Passport.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? PersonName { get; set; }

        [ForeignKey("OrderId")]
        public SendOwnData? Order {get;set;}
        public int? OrderId {get;set;}


    }
}

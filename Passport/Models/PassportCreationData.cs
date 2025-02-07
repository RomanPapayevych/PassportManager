namespace Passport.Models
{
    public class PassportCreationData
    {
        public string? PassportNumber { get; set; }  // Номер паспорта
        public string? IdentificationCode { get; set; }  // Ідентифікаційний код
        public DateTime IssueDate { get; set; }  // Дата видачі
        public DateTime ExpiryDate { get; set; }  // Дата закінчення дії
        public string? IssuingAuthority { get; set; }  // Орган видачі
    }
}

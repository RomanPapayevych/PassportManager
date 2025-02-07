namespace Passport.Models
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}

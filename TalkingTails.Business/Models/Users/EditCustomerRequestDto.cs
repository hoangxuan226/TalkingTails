namespace TalkingTails.Business.Models.Users
{
    public class EditCustomerRequestDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Email { get; set; }
    }
}
using Microsoft.AspNetCore.Http;

namespace TalkingTails.Business.Models.Organizations
{
    public class UpdateRequestDto
    {
        public required string Id { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public required string Description { get; set; }
        public string? MeetLink { get; set; }
    }
}
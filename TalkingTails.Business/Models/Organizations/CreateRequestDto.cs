using Microsoft.AspNetCore.Http;
using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Organizations
{
    public class CreateRequestDto
    {
        public IFormFile? ProfileImage { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public OrganizationStatus Status { get; set; }
        public required string Address { get; set; }
        public required string Description { get; set; }
        public required string Password { get; set; }
    }
}
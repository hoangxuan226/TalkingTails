using TalkingTails.API.Models.Users;
using TalkingTails.Business.Models.Users;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class UserMapping
    {
        public static EditCustomerRequestDto ToEditCustomerRequestDto(this EditCustomerRequest request, string userId)
        {
            return new EditCustomerRequestDto
            {
                Id = userId,
                Name = request.Name,
                Address = request.Address,
                Birthday = request.Birthday,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };
        }
    }
}
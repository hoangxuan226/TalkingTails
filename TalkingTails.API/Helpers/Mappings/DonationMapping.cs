using TalkingTails.API.Models.Donations;
using TalkingTails.Business.Models.Donations;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class DonationMapping
    {
        public static CreateCheckoutRequestDto ToCreateCheckoutRequestDto(this CreateCheckoutRequest request,
            string donorId)
        {
            return new CreateCheckoutRequestDto
            {
                DonorId = donorId,
                OrganizationId = request.OrganizationId,
                DonationPackageId = request.DonationPackageId,
                Message = request.Message ?? string.Empty,
                ReturnUrl = request.ReturnUrl,
                CancelUrl = request.CancelUrl
            };
        }
    }
}
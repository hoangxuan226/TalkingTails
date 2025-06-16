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

        public static CustomerDonationListRequestDto ToCustomerDonationListRequestDto(
            this CustomerDonationListRequest request, string userId)
        {
            return new CustomerDonationListRequestDto
            {
                UserId = userId,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public static AdminDonationListRequestDto ToAdminDonationListRequestDto(
            this AdminDonationListRequest request)
        {
            return new AdminDonationListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                FilterByStartDate = request.FilterByStartDate,
                FilterByEndDate = request.FilterByEndDate,
                SearchByPackageName = request.SearchByPackageName,
                Sort = request.Sort
            };
        }
    }
}
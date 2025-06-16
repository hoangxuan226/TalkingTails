using Net.payOS.Types;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Donations;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IDonationService
    {
        Task<OneOf<string, IError>> CreateCheckoutUrl(CreateCheckoutRequestDto checkoutRequest);
        Task<bool> PayOsTransferHandler(WebhookType webhookType);
        Task<List<TopDonorsDto>> GetTopDonorsAsync(int count);

        Task<Pagination<CustomerDonationBasicDto>> GetCustomerDonationHistoryAsync(
            CustomerDonationListRequestDto requestDto);

        Task<Pagination<AdminDonationBasicDto>> GetAdminDonationHistoryAsync(AdminDonationListRequestDto requestDto);
        Task<StatisticalDto> GetDonationStatisticsAsync(DateTime? startDate, DateTime? endDate);
    }
}
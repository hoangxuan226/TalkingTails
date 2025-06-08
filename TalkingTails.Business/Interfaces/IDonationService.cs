using Net.payOS.Types;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Donations;

namespace TalkingTails.Business.Interfaces
{
    public interface IDonationService
    {
        Task<OneOf<string, IError>> CreateCheckoutUrl(CreateCheckoutRequestDto checkoutRequest);
        Task<bool> PayOsTransferHandler(WebhookType webhookType);
    }
}
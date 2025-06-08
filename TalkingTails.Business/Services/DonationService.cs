using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Donations;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class DonationService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IDateTimeProvider dateTimeProvider,
        PayOS payOs,
        ILogger<DonationService> logger) : IDonationService
    {
        private const string PayOsStatusPending = "PENDING";
        private const string PayOsStatusPaid = "PAID";
        private const string PayOsStatusFailed = "FAILED";

        public async Task<OneOf<string, IError>> CreateCheckoutUrl(CreateCheckoutRequestDto checkoutRequest)
        {
            // Validate the donor
            var donor = await userManager.FindByIdAsync(checkoutRequest.DonorId);
            if (donor == null || !await userManager.IsInRoleAsync(donor, nameof(Roles.Customer)))
            {
                return new InvalidResourcesError();
            }

            // Validate the organization
            var organization = await userManager.FindByIdAsync(checkoutRequest.OrganizationId);
            if (organization == null || !await userManager.IsInRoleAsync(organization, nameof(Roles.Organization)))
            {
                return new InvalidResourcesError();
            }

            // Validate the donation package
            var donationPackage = await unitOfWork.GenericRepository<DonationPackage>()
                .GetAsync(dp => dp.Id == checkoutRequest.DonationPackageId);
            if (donationPackage == null)
            {
                return new InvalidResourcesError();
            }

            var amount = donationPackage.Amount;
            //var expireAt = dateTimeProvider.UtcNowOffset.AddMinutes(payOsSettings.ExpireInMinutes).ToUnixTimeSeconds();

            var oldLink = await unitOfWork.GenericRepository<DonationLinkRequest>()
                .GetAsync(dlr =>
                    dlr.Amount == amount && dlr.ReturnUrl.Equals(checkoutRequest.ReturnUrl) &&
                    dlr.CancelUrl.Equals(checkoutRequest.CancelUrl) &&
                    dlr.UserId.Equals(checkoutRequest.DonorId) &&
                    dlr.OrganizationId.Equals(checkoutRequest.OrganizationId) &&
                    dlr.Status.Equals(PayOsStatusPending));
            if (oldLink != null)
            {
                PaymentLinkInformation currLinkInfo;
                try
                {
                    currLinkInfo = await payOs.getPaymentLinkInformation(oldLink.OrderCode);
                }
                catch (Exception ex)
                {
                    return new PayOsError(ex);
                }

                // If the link is still usable, update its information and return that link
                if (currLinkInfo.status.Equals(PayOsStatusPending))
                {
                    if (oldLink.PackageName != donationPackage.Name || oldLink.Message != checkoutRequest.Message)
                    {
                        oldLink.PackageName = donationPackage.Name;
                        oldLink.Message = checkoutRequest.Message;
                        oldLink.UpdatedAt = dateTimeProvider.UtcNow;
                        unitOfWork.GenericRepository<DonationLinkRequest>()
                            .Update(oldLink);
                        await unitOfWork.SaveChangesAsync();
                    }

                    return oldLink.CheckoutUrl;
                }

                // If the link is not usable, update its status, save changes at the end of the method
                oldLink.Status = currLinkInfo.status;
                oldLink.UpdatedAt = dateTimeProvider.UtcNow;
                unitOfWork.GenericRepository<DonationLinkRequest>()
                    .Update(oldLink);
            }

            // If there is no old usable link
            // Create a new donation link request
            var paymentLinkRequest = new PaymentData(
                orderCode: int.Parse(dateTimeProvider.UtcNowOffset.ToString("ffffff")),
                amount: amount,
                description: $"UNGHO{amount}",
                items: [],
                returnUrl: checkoutRequest.ReturnUrl,
                cancelUrl: checkoutRequest.CancelUrl
                //expiredAt: (Int32)expireAt
            );

            CreatePaymentResult response;
            try
            {
                response = await payOs.createPaymentLink(paymentLinkRequest);
            }
            catch (Exception ex)
            {
                return new PayOsError(ex);
            }

            // Save the new link request to the database
            var newLinkRequest = new DonationLinkRequest
            {
                OrderCode = response.orderCode,
                Amount = response.amount,
                Description = response.description,
                ReturnUrl = checkoutRequest.ReturnUrl,
                CancelUrl = checkoutRequest.CancelUrl,
                CheckoutUrl = response.checkoutUrl,
                UserId = checkoutRequest.DonorId,
                OrganizationId = checkoutRequest.OrganizationId,
                PackageName = donationPackage.Name,
                Message = checkoutRequest.Message,
                UpdatedAt = dateTimeProvider.UtcNow,
                Status = response.status
            };
            await unitOfWork.GenericRepository<DonationLinkRequest>()
                .InsertAsync(newLinkRequest);
            await unitOfWork.SaveChangesAsync();
            return response.checkoutUrl;
        }

        public async Task<bool> PayOsTransferHandler(WebhookType webhookType)
        {
            try
            {
                var data = payOs.verifyPaymentWebhookData(webhookType);
                var donationLinkRequest = await unitOfWork.GenericRepository<DonationLinkRequest>()
                    .GetAsync(dlr => dlr.OrderCode == data.orderCode);
                if (donationLinkRequest != null)
                {
                    if (data.code.Equals("00"))
                    {
                        var donation = new Donation
                        {
                            Amount = donationLinkRequest.Amount,
                            UserId = donationLinkRequest.UserId,
                            OrganizationId = donationLinkRequest.OrganizationId,
                            PackageName = donationLinkRequest.PackageName,
                            Message = donationLinkRequest.Message,
                            CreatedAt = dateTimeProvider.UtcNow,
                            DonationLinkRequestId = donationLinkRequest.Id
                        };
                        await unitOfWork.GenericRepository<Donation>()
                            .InsertAsync(donation);
                        donationLinkRequest.Status = PayOsStatusPaid;
                    }
                    else
                    {
                        donationLinkRequest.Status = PayOsStatusFailed;
                    }

                    donationLinkRequest.UpdatedAt = dateTimeProvider.UtcNow;
                    unitOfWork.GenericRepository<DonationLinkRequest>()
                        .Update(donationLinkRequest);
                    await unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error PayOs webhook data");
            }

            return false;
        }
    }
}
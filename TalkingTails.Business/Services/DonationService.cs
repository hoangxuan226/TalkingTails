using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Donations;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
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
                    dlr.UserId == checkoutRequest.DonorId &&
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
                        // Save successful donation
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

                        // Increase organization's donation amount
                        var organization = await userManager.FindByIdAsync(donationLinkRequest.OrganizationId);
                        if (organization != null)
                        {
                            organization.Organization!.TotalDonationAmount += donation.Amount;
                            await userManager.UpdateAsync(organization);
                        }

                        // Increase donor's donation amount
                        if (!string.IsNullOrEmpty(donationLinkRequest.UserId))
                        {
                            var donor = await userManager.FindByIdAsync(donationLinkRequest.UserId);
                            if (donor != null)
                            {
                                donor.Customer!.TotalDonatedAmount += donation.Amount;
                                await userManager.UpdateAsync(donor);
                            }
                        }
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

        public Task<List<TopDonorsDto>> GetTopDonorsAsync(int count)
        {
            count = Math.Max(count, 0);
            return unitOfWork.GenericRepository<ApplicationUser>().GetQueryable()
                .Where(u => u.Customer.TotalDonatedAmount > 0)
                .OrderByDescending(u => u.Customer.TotalDonatedAmount)
                .Take(count)
                .Select(u => new TopDonorsDto
                {
                    Name = u.Name ?? u.UserName ?? u.Email ?? string.Empty,
                    ProfileImage = u.ProfileImage,
                    TotalDonatedAmount = u.Customer.TotalDonatedAmount
                })
                .ToListAsync();
        }

        public Task<Pagination<CustomerDonationBasicDto>> GetCustomerDonationHistoryAsync(
            CustomerDonationListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            Expression<Func<Donation, bool>> filter = d => d.UserId == requestDto.UserId;
            var sort = $"{nameof(Donation.CreatedAt)} desc";
            return unitOfWork.GenericRepository<Donation>()
                .GetPaginationAsync<CustomerDonationBasicDto>(pageIndex, pageSize, null, filter, sort);
        }

        public Task<Pagination<AdminDonationBasicDto>> GetAdminDonationHistoryAsync(
            AdminDonationListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            var sort = requestDto.Sort ?? $"{nameof(Donation.CreatedAt)} desc";
            Expression<Func<Donation, bool>> filter = d =>
                (requestDto.FilterByStartDate == null || d.CreatedAt >= requestDto.FilterByStartDate)
                && (requestDto.FilterByEndDate == null || d.CreatedAt <= requestDto.FilterByEndDate)
                && (requestDto.SearchByPackageName == null || d.PackageName.Contains(requestDto.SearchByPackageName));
            return unitOfWork.GenericRepository<Donation>()
                .GetPaginationAsync<AdminDonationBasicDto>(pageIndex, pageSize, null, filter, sort);
        }

        public async Task<StatisticalDto> GetDonationStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var packageStats = await unitOfWork.GenericRepository<Donation>()
                .GetQueryable()
                .Where(d => (startDate == null || d.CreatedAt >= startDate) &&
                            (endDate == null || d.CreatedAt <= endDate))
                .GroupBy(d => d.PackageName)
                .Select(g => new PackageStatistics
                {
                    PackageName = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    Count = g.Count()
                })
                .ToListAsync();

            return new StatisticalDto
            {
                PackageStatistics = packageStats
            };
        }
    }
}
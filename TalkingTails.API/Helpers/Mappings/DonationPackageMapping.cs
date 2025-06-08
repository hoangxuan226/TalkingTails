using TalkingTails.API.Models.DonationPackages;
using TalkingTails.Repository.Entities;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class DonationPackageMapping
    {
        public static PackageResponse ToPackageResponse(this DonationPackage donationPackage)
        {
            return new PackageResponse
            {
                Id = donationPackage.Id,
                Name = donationPackage.Name,
                Description = donationPackage.Description,
                Amount = donationPackage.Amount,
                Status = donationPackage.Status.ToString(),
                CreatedAt = donationPackage.CreatedAt,
                UpdatedAt = donationPackage.UpdatedAt
            };
        }

        public static List<PackageResponse> ToPackageResponseList(this List<DonationPackage> donationPackages)
        {
            return donationPackages.Select(dp => dp.ToPackageResponse()).ToList();
        }
    }
}
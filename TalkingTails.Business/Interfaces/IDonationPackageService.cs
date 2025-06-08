using TalkingTails.Business.Models.DonationPackages;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Interfaces
{
    public interface IDonationPackageService
    {
        Task<List<DonationPackage>> GetAllAsync();
        Task<List<PackageBasicDto>> GetActivePackageBasicListAsync();
    }
}
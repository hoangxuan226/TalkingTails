using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.DonationPackages;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class DonationPackageService(IUnitOfWork unitOfWork) : IDonationPackageService
    {
        public Task<List<DonationPackage>> GetAllAsync()
        {
            return unitOfWork.GenericRepository<DonationPackage>().GetAllAsync();
        }

        public Task<List<PackageBasicDto>> GetActivePackageBasicListAsync()
        {
            return unitOfWork.GenericRepository<DonationPackage>()
                .GetAllAsync<PackageBasicDto>(x => x.Status == DonationPackageStatus.Active);
        }
    }
}
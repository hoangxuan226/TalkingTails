using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Organizations;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class OrganizationService(IUnitOfWork unitOfWork)
        : IOrganizationService
    {
        public Task<List<OrganizationBasicDto>> GetAllAsync()
        {
            var repository = (IApplicationUserRepository)unitOfWork.GenericRepository<ApplicationUser>();
            repository.ApplyRole(Roles.Organization);
            return repository.GetAllAsync<OrganizationBasicDto>();
        }
    }
}
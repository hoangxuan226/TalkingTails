using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Repository.Interfaces
{
    public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
    {
        void ApplyRole(Roles role);
    }
}
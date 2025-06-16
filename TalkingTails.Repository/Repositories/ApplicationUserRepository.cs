using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Data;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Repository.Repositories
{
    public class ApplicationUserRepository(ApplicationDbContext context)
        : GenericRepository<ApplicationUser>(context), IApplicationUserRepository
    {
        private Roles? _role;

        public void ApplyRole(Roles role)
        {
            _role = role;
        }

        public override IQueryable<ApplicationUser> GetQueryable()
        {
            var query = base.GetQueryable();
            if (_role != null)
            {
                query = query
                    .Join(DbContext.UserRoles, users => users.Id, userRoles => userRoles.UserId,
                        (users, userRoles) => new { users, userRoles })
                    .Join(DbContext.Roles, join1 => join1.userRoles.RoleId, roles => roles.Id,
                        (join1, roles) => new { join1.users, roles })
                    .Where(join2 => join2.roles.Name == _role.ToString())
                    .Select(join2 => join2.users);
            }

            return query;
        }
    };
}
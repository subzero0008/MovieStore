using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieStoreMvc.Models.Domain;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class UserRepository
    {
        private readonly DatabaseContext _dbContext;

        public UserRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ApplicationUser> GetNonAdminUsers()
        {
            return _dbContext.Users.Where(u => !_dbContext.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == "adminRoleId"));
        }
    }
}

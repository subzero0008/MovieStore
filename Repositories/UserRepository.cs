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

        // Метод за вземане на всички потребители (без филтриране по роли)
        public IQueryable<ApplicationUser> GetAllUsers()
        {
            return _dbContext.Users; // Вземаме всички потребители без филтриране по роли
        }
    }
}

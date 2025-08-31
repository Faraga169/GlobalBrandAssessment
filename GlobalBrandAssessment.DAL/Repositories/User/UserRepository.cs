using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public UserRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }
        public async Task<int?> GetEmployeeIdByUserIdAsync(int? userId)
        {
            return await globalbrandDbContext.Users.Where(u => u.UserId == userId).Select(u => u.EmployeeId).FirstOrDefaultAsync();
        }
        public async Task<int> AddAsync(User user)
        {
            globalbrandDbContext.Users.Add(user);
            return await globalbrandDbContext.SaveChangesAsync();
        }
    }
}

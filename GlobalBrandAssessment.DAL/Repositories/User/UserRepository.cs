using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public UserRepository(GlobalbrandDbContext globalbrandDbContext) : base(globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }
        public async Task<int?> GetEmployeeIdByUserIdAsync(int? userId)
        {
            return await globalbrandDbContext.Users.Where(u => u.UserId == userId).Select(u => u.EmployeeId).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(int? id)
        {
            var user=globalbrandDbContext.Users.FirstOrDefault(u => u.EmployeeId == id);
            if (user != null)
            {
                globalbrandDbContext.Users.Remove(user);
                 await Task.CompletedTask;
            }
           

        }
    }
}

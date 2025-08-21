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
        public int? GetEmployeeIdByUserId(int? userId)
        {
            return globalbrandDbContext.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.EmployeeId)
                .FirstOrDefault();
        }
        public int Add(User user)
        {
            globalbrandDbContext.Users.Add(user);
            return globalbrandDbContext.SaveChanges();
        }
    }
}

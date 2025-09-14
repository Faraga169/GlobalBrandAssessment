using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IUserService
    {
        public Task<int?> GetEmployeeIdByUserIdAsync(int? userId);
        public Task<int?> AddAsync(User user);
        public  Task<int> RemoveAsync(int? id);
        
    }
}

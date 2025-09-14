using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IUserRepository :IGenericRepository<User>
    {
        public Task<int?> GetEmployeeIdByUserIdAsync(int? userId);
        public Task RemoveAsync(int? id);

    }
}

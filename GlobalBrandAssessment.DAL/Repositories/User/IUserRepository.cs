using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IUserRepository
    {
        public int Add(User user);
        public int? GetEmployeeIdByUserId(int? userId);
       
    }
}

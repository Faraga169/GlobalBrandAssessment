using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IUserService
    {
        public int? GetEmployeeIdByUserId(int? userId);
        public int Add(User user);
    }
}

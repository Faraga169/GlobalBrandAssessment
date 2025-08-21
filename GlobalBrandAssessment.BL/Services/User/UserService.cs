using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public int? GetEmployeeIdByUserId(int? userId)
        {
            return userRepository.GetEmployeeIdByUserId(userId);
        }

        public int Add(User user)
        {
            if (user == null) { 
            return 0;
            }
            return userRepository.Add(user);
        }
    }
}

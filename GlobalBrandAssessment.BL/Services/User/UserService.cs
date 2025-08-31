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

        public Task<int?> GetEmployeeIdByUserIdAsync(int? userId)
        {
            return userRepository.GetEmployeeIdByUserIdAsync(userId);
        }

        public async Task<int> AddAsync(User user)
        {
            if (user == null) { 
            return 0;
            }
            return await userRepository.AddAsync(user);
        }
    }
}

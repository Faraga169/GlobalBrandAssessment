using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.UnitofWork;

namespace GlobalBrandAssessment.BL.Services
{
    public class UserService:IUserService
    {
      
        private readonly IUnitofWork unitofWork;

        public UserService(IUnitofWork unitofWork)
        {
            this.unitofWork = unitofWork;
        }

        public Task<int?> GetEmployeeIdByUserIdAsync(int? userId)
        {
            return unitofWork.userRepository.GetEmployeeIdByUserIdAsync(userId);
        }

        public async Task<int?> AddAsync(User user)
        {
            if (user == null) { 
            return 0;
            }
           await unitofWork.userRepository.AddAsync(user);
          return await unitofWork.CompleteAsync();
           
          

        }

        public async Task<int> RemoveAsync(int? id)
        {
            
            await unitofWork.userRepository.RemoveAsync(id);
            var result= await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;

        }


    }
}

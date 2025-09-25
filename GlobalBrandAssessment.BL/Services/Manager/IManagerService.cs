using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services.Manager
{
    public interface IManagerService :IGenericService<Employee,AddAndUpdateManagerDTO>
    {

       public Task<int>Add(AddAndUpdateManagerDTO addAndUpdateManagerDTO);

        public Task<int> DemoteManagerToEmployeeAsync(int? managerId);
        public Task<int> DeleteAsync(int? id);

        public Task<List<GetAllAndSearchManagerDTO>> SearchAsync(string searchname,int?managerid);



        public Task<List<Employee>> GetAllManagersAsync();

        public Task<Employee?> GetManagerByDepartmentIdAsync(int? deptId);


       
    }
}

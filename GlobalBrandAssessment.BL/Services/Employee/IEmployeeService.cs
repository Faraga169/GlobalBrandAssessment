using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IEmployeeService: IGenericService<AddAndUpdateManagerDTO>
    {
        public Task<Employee> GetEmployeeByIdAsync(int? employeeId);

        public Task<List<GetAllAndSearchManagerDTO>> GetEmployeesByManagerAsync(int? ManagerId);
    }
}

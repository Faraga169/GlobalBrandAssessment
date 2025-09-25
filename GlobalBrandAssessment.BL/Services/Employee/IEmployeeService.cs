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
    public interface IEmployeeService: IGenericService<Employee,AddAndUpdateManagerDTO>
    {
        public Task<Employee?> GetEmployeeByIdAsync(int? employeeId);

        public Task<PagedResult<GetAllAndSearchManagerDTO>> GetEmployeesByManagerPagedAsync(int? ManagerId, int pageno=1, int pagesize = 5, string sortcolumn="FirstName");

        public Task<PagedResult<GetAllAndSearchManagerDTO>> GetAllPagedAsync( int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName");

        public Task<string?> GetEmployeeImageUrlAsync(int id);

        public Task<List<GetAllAndSearchManagerDTO>> GetAll();

        public Task<List<GetAllAndSearchManagerDTO>> GetEmployeesByManagerId(int? managerid);

        public Task<List<GetAllAndSearchManagerDTO>> GetEmployeesByDeptId(int id);
    }
}

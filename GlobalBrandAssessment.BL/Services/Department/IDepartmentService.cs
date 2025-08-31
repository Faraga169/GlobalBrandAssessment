using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IDepartmentService: IGenericService<AddAndUpdateDepartmentDTO>
    {
        public Task<List<GetAllandSearchDepartmentDTO>> GetAllAsync();


        public Task<int> DeleteAsync(int? id);

        public Task<List<GetAllandSearchDepartmentDTO>> SearchAsync(string searchname);

        public Task<AddAndUpdateDepartmentDTO> GetDepartmentByIdAsync(int? id);
       
    }
}

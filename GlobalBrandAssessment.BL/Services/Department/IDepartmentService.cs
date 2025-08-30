using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IDepartmentService
    {
        public List<GetAllandSearchDepartmentDTO> GetAll();

        public int Add(AddAndUpdateDepartmentDTO department);

        public int Update(AddAndUpdateDepartmentDTO department);

        public int Delete(int? id);

        public List<GetAllandSearchDepartmentDTO> Search(string searchname);

        public AddAndUpdateDepartmentDTO GetDepartmentById(int? id);
       
    }
}

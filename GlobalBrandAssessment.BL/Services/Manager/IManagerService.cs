using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services.Manager
{
    public interface IManagerService
    {


        public int Add(AddAndUpdateManagerDTO employee);

        public int Update(AddAndUpdateManagerDTO employee);

        public int Delete(int? id);

        public List<GetAllAndSearchManagerDTO> Search(string searchname, int? managerid);



        public List<Employee> GetAllManagers();

        public Employee GetManagerByDepartmentId(int? deptId);
       
    }
}

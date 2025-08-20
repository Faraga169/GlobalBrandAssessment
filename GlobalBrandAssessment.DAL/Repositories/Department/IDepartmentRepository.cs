using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IDepartmentRepository
    {
        public List<Department> GetAll();

        public int Add(Department department);

        public int Update(Department department);

        public int Delete(int? id);

        public List<Department> Search(string searchname);

        public Department GetDepartmentById(int? id);


        
    }
}

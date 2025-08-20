using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.GlobalBrandDbContext;


public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository departmentrepository;

    public DepartmentService(IDepartmentRepository departmentrepository)
    {

        this.departmentrepository = departmentrepository;
    }

    public int Add(Department department)
    {
        return departmentrepository.Add(department);
    }

    public int Delete(int? id)
    {
        return departmentrepository.Delete(id);
    }


    public List<Department> Search(string searchname)
    {
        return departmentrepository.Search(searchname);
    }
    public int Update(Department department)
    {
        return departmentrepository.Update(department);
    }


    public List<Department> GetAll()
    {
        return departmentrepository.GetAll();
    }

    

    public Department GetDepartmentById(int? id)
    {
        return departmentrepository.GetDepartmentById(id);
    }
}

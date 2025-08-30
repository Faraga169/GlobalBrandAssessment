using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.GlobalBrandDbContext;


public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository departmentrepository;
    private readonly IMapper mapper;

    public DepartmentService(IDepartmentRepository departmentrepository,IMapper mapper)
    {

        this.departmentrepository = departmentrepository;
        this.mapper = mapper;
    }

    public int Add(AddAndUpdateDepartmentDTO department)
    {
        var result=mapper.Map<AddAndUpdateDepartmentDTO, Department>(department);
        return departmentrepository.Add(result);
    }

    public int Delete(int? id)
    {
        return departmentrepository.Delete(id);
    }


    public List<GetAllandSearchDepartmentDTO> Search(string searchname)
    {
        var departmentlist = departmentrepository.Search(searchname);
        var SearchDepartmentDTO = mapper.Map<List<Department>, List<GetAllandSearchDepartmentDTO>>(departmentlist);
        return SearchDepartmentDTO;
    }
    public int Update(AddAndUpdateDepartmentDTO department)
    {
        var result = mapper.Map<AddAndUpdateDepartmentDTO, Department>(department);
        return departmentrepository.Update(result);
    }


    public List<GetAllandSearchDepartmentDTO> GetAll()
    {
        var departmentlist = departmentrepository.GetAll();
        var GetAllDepartmentDTO = mapper.Map<List<Department>, List<GetAllandSearchDepartmentDTO>>(departmentlist);
        return GetAllDepartmentDTO;
    }

    

    public AddAndUpdateDepartmentDTO GetDepartmentById(int? id)
    {
        var department = departmentrepository.GetDepartmentById(id);
        var GetDepartmentDTO = mapper.Map<Department, AddAndUpdateDepartmentDTO>(department);
        return GetDepartmentDTO ;
    }
}

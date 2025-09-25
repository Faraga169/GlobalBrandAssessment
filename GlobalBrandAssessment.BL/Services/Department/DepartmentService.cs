using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;



public class DepartmentService : GenericService<Department,AddAndUpdateDepartmentDTO>, IDepartmentService


{
    private readonly IUnitofWork unitofWork;
  
    private readonly IMapper mapper;

    public DepartmentService(IUnitofWork unitofWork, IMapper mapper) : base(unitofWork, mapper)
    {
        this.unitofWork = unitofWork;
   
        this.mapper = mapper;
    }


    public async Task<int> DeleteAsync(int? id)
    {
        if (id == null)
            return 0;

        
        var department = await unitofWork.departmentRepository.GetDepartmentById(id);

        if (department == null)
            return 0;


        if (department.Employees != null && department.Employees.Any(e => e.Id != department.ManagerId))
        {
           
            return 0;
        }
       
        if (department.Manager != null)
        {
            department.Manager.DeptId = null; 
            department.Manager = null;
            department.ManagerId = null;
        }


        await unitofWork.departmentRepository.DeleteAsync(department);

        var result = await unitofWork.CompleteAsync();

        return result > 0 ? result : -1; 
    }

    public async Task<List<GetAllandSearchDepartmentDTO>> GetDepartmentByManagerId(int? managerId) {

        var department = await unitofWork.departmentRepository.GetDepartmentsByManagerId(managerId);
        
        var GetDepartmentDTO = mapper.Map<List<Department>, List<GetAllandSearchDepartmentDTO>>(department);
        return GetDepartmentDTO;
    }

    public async Task<List<GetAllandSearchDepartmentDTO>> SearchAsync(string searchname)
    {
       
        var departmentlist = await unitofWork.departmentRepository.SearchAsync(searchname);
        var SearchDepartmentDTO = mapper.Map<List<Department>, List<GetAllandSearchDepartmentDTO>>(departmentlist);
        return SearchDepartmentDTO;
    }


    public async Task< List<GetAllandSearchDepartmentDTO>> GetAllAsync()
    {
        var departmentlist = await unitofWork.departmentRepository.GetAllAsync();
        var GetAllDepartmentDTO = mapper.Map<List<Department>, List<GetAllandSearchDepartmentDTO>>(departmentlist);
        return GetAllDepartmentDTO;
    }

    

    public async Task<AddAndUpdateDepartmentDTO?> GetDepartmentByIdAsync(int? id)
    {
        var department =await unitofWork.departmentRepository.GetDepartmentById(id);
        if (department is null)
            return null;
        var GetDepartmentDTO = mapper.Map<Department, AddAndUpdateDepartmentDTO>(department);
        return GetDepartmentDTO ;
    }

   
}

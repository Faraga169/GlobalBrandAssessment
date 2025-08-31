using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;



public class DepartmentService : GenericService<Department>,IDepartmentService


{
    private readonly IDepartmentRepository departmentRepository;
    private readonly IMapper mapper;

    public DepartmentService(IDepartmentRepository genericRepository,IMapper mapper): base(genericRepository)
    {
        this.departmentRepository = genericRepository;
        this.mapper = mapper;
    }
    public async Task<int> AddAsync(AddAndUpdateDepartmentDTO department)
    {
        var result=mapper.Map<AddAndUpdateDepartmentDTO, Department>(department);
        return await departmentRepository.AddAsync(result);
    }

    public async Task<int> DeleteAsync(int? id)
    {
        if (id == null)
            return 0;

       
           

        return await departmentRepository.DeleteAsync(id);
    }


    public async Task<List<GetAllandSearchDepartmentDTO>> SearchAsync(string searchname)
    {
        var departmentlist = await departmentRepository.SearchAsync(searchname);
        var SearchDepartmentDTO = mapper.Map<List<Department>, List<GetAllandSearchDepartmentDTO>>(departmentlist);
        return  SearchDepartmentDTO;
    }
    public async Task<int> UpdateAsync(AddAndUpdateDepartmentDTO department)
    {
        var result = mapper.Map<AddAndUpdateDepartmentDTO, Department>(department);
        return await departmentRepository.UpdateAsync(result);
    }


    public async Task< List<GetAllandSearchDepartmentDTO>> GetAllAsync()
    {
        var departmentlist = await departmentRepository.GetAllAsync();
        var GetAllDepartmentDTO = mapper.Map<List<Department>, List<GetAllandSearchDepartmentDTO>>(departmentlist);
        return GetAllDepartmentDTO;
    }

    

    public async Task<AddAndUpdateDepartmentDTO> GetDepartmentByIdAsync(int? id)
    {
        var department =await departmentRepository.GetDepartmentById(id);
        var GetDepartmentDTO = mapper.Map<Department, AddAndUpdateDepartmentDTO>(department);
        return GetDepartmentDTO ;
    }

   
}

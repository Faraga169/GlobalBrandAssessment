using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.GlobalBrandDbContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GlobalBrandAssessment.BL.Services.Manager
{
    public class ManagerService : IManagerService
    {
        
        private readonly IManagerRepository managerRepository;
        private readonly IMapper mapper;

        public ManagerService(IManagerRepository managerRepository,IMapper mapper)
        {
            
            this.managerRepository = managerRepository;
            this.mapper = mapper;
        }
        public Task<int> AddAsync(AddAndUpdateManagerDTO employee)
        {
            var result = mapper.Map<AddAndUpdateManagerDTO, Employee>(employee);

            //var result = managerRepository.Add(new Employee()
            //{
            //    FirstName = employee.FirstName,
            //    LastName = employee.LastName,
            //    Salary = employee.Salary,
            //    Password = employee.Password,
            //    ImageURL = employee.ImageURL,
            //    DeptId = employee.DeptId,
            //});
                
            return managerRepository.AddAsync(result);
        }

        public Task<int> DeleteAsync(int? id)
        {
            return managerRepository.DeleteAsync(id);
        }

       

        public async Task<List<GetAllAndSearchManagerDTO>> SearchAsync(string searchname, int? managerid)
        {
            var managerlist = await managerRepository.SearchAsync(searchname, managerid);
            var SearchManagerDTO= mapper.Map<List<Employee>,List<GetAllAndSearchManagerDTO>>(managerlist);

            //var result = managerlist.Select(m => new SearchManagerDTO
            //{
            //    FirstName = m.FirstName,
            //    LastName = m.LastName,
            //    Salary = m.Salary,
            //    ImageURL = m.ImageURL,
            //    Department = m.Department.Name
            //}).ToList();

            return SearchManagerDTO;
        }

        public Task<int> UpdateAsync(AddAndUpdateManagerDTO employee)
        {
            var result = mapper.Map<AddAndUpdateManagerDTO, Employee>(employee);
            return managerRepository.UpdateAsync(result);
        }

       

        public Task<Employee> GetManagerByDepartmentIdAsync(int? deptId)
        {
            return managerRepository.GetManagerByDepartmentIdAsync(deptId) ;
        }

        public Task<List<Employee>> GetAllManagersAsync()
        {
            return managerRepository.GetAllManagersAsync();
        }

       
    }
}
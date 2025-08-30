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
        public int Add(AddAndUpdateManagerDTO employee)
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
                
            return managerRepository.Add(result);
        }

        public int Delete(int? id)
        {
            return managerRepository.Delete(id);
        }

       

        public List<GetAllAndSearchManagerDTO> Search(string searchname, int? managerid)
        {
            var managerlist = managerRepository.Search(searchname, managerid);
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

        public int Update(AddAndUpdateManagerDTO employee)
        {
            var result = mapper.Map<AddAndUpdateManagerDTO, Employee>(employee);
            return managerRepository.Update(result);
        }

       

        public Employee GetManagerByDepartmentId(int? deptId)
        {
            return managerRepository.GetManagerByDepartmentId(deptId) ;
        }

        public List<Employee> GetAllManagers()
        {
            return managerRepository.GetAllManagers();
        }

       
    }
}
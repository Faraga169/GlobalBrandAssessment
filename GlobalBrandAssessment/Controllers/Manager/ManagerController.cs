using GlobalBrandAssessment.BL.Services.Manager;

using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.BL.Services;
using System.Threading.Tasks;
using GlobalBrandAssessment.GlobalBrandDbContext;
using System.Data;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GlobalBrandAssessment.DAL.Data.Models.Common;

namespace GlobalBrandAssessment.PL.Controllers.Employee
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly IManagerService managerService;
        private readonly UserManager<User> userManager;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IMapper mapper;
        private readonly ILogger<ManagerController> logger;
        private readonly IWebHostEnvironment environment;

        public ManagerController(IManagerService managerService, UserManager<User> userManager, IEmployeeService employeeService, IDepartmentService departmentService, IMapper mapper, ILogger<ManagerController> logger,IWebHostEnvironment environment)
        {
            this.managerService = managerService;
            this.userManager = userManager;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.mapper = mapper;
            this.logger = logger;
            this.environment = environment;
        }

        [HttpGet]
        
        public async Task<IActionResult> Index(int pageno=1,int pagesize=5,string sortcolumn="FirstName")

        {

             
            try
            {

                var currentUser = await userManager.GetUserAsync(User);
                var managerId = currentUser?.EmployeeId;

                var manager = await employeeService.GetEmployeesByManagerPagedAsync(managerId,pageno,pagesize,sortcolumn);
                return View(manager);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "An error occurred in Manager Index action.");
                return PartialView("Errorpartial", ex);
            }

        }



        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            try
            {
                var currentUser = await userManager.GetUserAsync(User);
                var managerId = currentUser?.EmployeeId;
                var manager=new List<GetAllAndSearchManagerDTO>();
                if (User.IsInRole("Manager"))
                {
                    manager = await managerService.SearchAsync(searchname, managerId);
                }

                else if (User.IsInRole("Admin")) {
                    manager = await managerService.SearchAsync(searchname,null);
                }
              
                if (manager == null || !manager.Any())
                
                    return PartialView("_IndexManagerPartial", new List<GetAllAndSearchManagerDTO>());
                

                return PartialView("_IndexManagerPartial", manager);


            }
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    TempData["Message"] = ex.Message;
                    return PartialView("_IndexManagerPartial", new List<GetAllAndSearchManagerDTO>());
                }
                else
                {
                    //2. Deployment => Log Error In File | Table in Database And Return Error View
                    logger.LogError(ex.Message);
                    return PartialView("Errorpartial",ex);
                }


            }
      
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

           
            try
            {
                var employee=await employeeService.GetEmployeeByIdAsync(id);
                return View(employee);
            }
           
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    TempData["Message"] = ex.Message;
                    return View();
                }
                else
                {
                    //2. Deployment => Log Error In File | Table in Database And Return Error View
                    logger.LogError(ex.Message);
                    return PartialView("Errorpartial",ex);
                }


            }

        }



       

        
        

       

      

        
    }
}
         
        
    


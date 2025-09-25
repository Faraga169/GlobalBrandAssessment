using System.Linq.Expressions;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GlobalBrandAssessment.PL.Controllers.Department
{
    
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService departmentService;
        private readonly IManagerService managerService;
        private readonly ILogger<DepartmentController> logger;
        private readonly IWebHostEnvironment environment;
        private readonly IEmployeeService employeeService;
        private readonly UserManager<User> userManager;

        public DepartmentController(IDepartmentService departmentService,IManagerService managerService,ILogger<DepartmentController> logger,IWebHostEnvironment environment,IEmployeeService employeeService,UserManager<User> userManager)
        {
            this.departmentService = departmentService;
            this.managerService = managerService;
            this.logger = logger;
            this.environment = environment;
            this.employeeService = employeeService;
            this.userManager = userManager;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;
            var department= new List<GetAllandSearchDepartmentDTO>();
            try
            {
                if (User.IsInRole("Admin")) {
                    department = await departmentService.GetAllAsync();
                    
                }
                else if (User.IsInRole("Manager"))
                {
                    department = await departmentService.GetDepartmentByManagerId(managerId);
                   
                }
                return View(department);
            }

            catch (Exception ex) 
            {
                logger.LogError(ex, "Error happened while Get All department");
                return PartialView("Errorpartial", ex);
            }

        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string searchname)
        {
            

            try {
                var department = await departmentService.SearchAsync(searchname);
                if (department == null || !department.Any())
                {
                    return PartialView("_IndexDepartmentPartial", new List<GetAllandSearchDepartmentDTO>());
                }
                return PartialView("_IndexDepartmentPartial", department);
            }

            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    TempData["Message"] = ex.Message;
                    return PartialView("_IndexDepartmentPartial", new List<GetAllandSearchDepartmentDTO>());
                }
                else {
                    //2. Deployment => Log Error In File | Table in Database And Return Error View
                    logger.LogError(ex.Message);
                    return PartialView("Errorpartial", ex);
                }
               

            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public  IActionResult Create()
        {
              
                return View();
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddAndUpdateDepartmentDTO department)
        {
           
               

            var managers = await managerService.GetAllManagersAsync();

            if (ModelState.IsValid)
                {
                    try {

                    int result = await departmentService.AddAsync(department);
                        if (result > 0)
                        {
                            TempData["Message"] = "Department created successfully.";
                            return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                        }

                        else
                        {
                            TempData["Message"] = "Failed to create Department.";
                            return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                        }

                    } 

                    catch (Exception ex) {
                    if (environment.IsDevelopment())
                    {
                        // 1.Development => Log Error In Console and Return Same view with Error Message
                        ModelState.AddModelError(string.Empty, ex.Message);
                       
                       
                    }

                    else {

                        //2. Deployment => Log Error In File | Table in Database And Return Error View
                        logger.LogError(ex.Message);
                      return PartialView("Errorpartial", ex);
                    }
                  
                }

                }
              
                ViewBag._Manager = new SelectList(managers, "Id", "FirstName");
                return PartialView("_CreateDepartmentPartial", department);

            }
          
          
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task< IActionResult> Edit(int? id)
        {
            try {
               
                if(!id.HasValue)
                    return BadRequest(); //400


                var department = await departmentService.GetDepartmentByIdAsync(id);
                if (department == null)
                    return NotFound(); //404


                var managers = await managerService.GetAllManagersAsync();
                ViewBag._Manager = new SelectList(managers, "Id", "FullName");
                return View(department);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while Edit department");
                TempData["Message"] = "Something went wrong, please try again later.";
                return RedirectToAction("Index", "Department");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit([FromRoute]int id,AddAndUpdateDepartmentDTO department)
        {
            var managers = await managerService.GetAllManagersAsync();
            if (ModelState.IsValid)
                {
                    try {

                    int result = await departmentService.UpdateAsync(department);
                    if (result > 0)
                    {
                        TempData["Message"] = "Department updated successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to update Department.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                    }

                } 
                catch (Exception ex) {
                    if (environment.IsDevelopment())
                    {
                        // 1.Development => Log Error In Console and Return Same view with Error Message
                        ModelState.AddModelError(string.Empty, ex.Message);
                        
                        
                    }
                    else
                    {
                        //2. Deployment => Log Error In File | Table in Database And Return Error View
                        logger.LogError(ex, "Error happened while updating department {DepartmentId}", department.Id);
                        return PartialView("Errorpartial", ex);
                    }

                }
                    
                }
                ViewBag._Manager = new SelectList(managers, "Id", "FirstName", department.ManagerId); ;
                return PartialView("_EditDepartmentPartial", department);
            }

           
           
        

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            
            try
            {

                if (!id.HasValue) {
                    TempData["Message"] = "Invalid department ID.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
                   

                var result = await departmentService.DeleteAsync(id);
                if (result > 0)
                {
                    TempData["Message"] = "Department delete successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
                else
                {
                    TempData["Message"] = "You Cant Delete Because Department exist employees";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
            }

            catch (Exception ex) 
            {
                logger.LogError(ex, "Error happened while delete department");
                TempData["Message"] = "Something went wrong, please try again later.";
                return Json(new { success = false, redirectUrl = Url.Action("Index", "Department") });
               
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var employees = await employeeService.GetEmployeesByDeptId(id);

               

                return View(employees);
            }
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                   
                    return View("Details", new List<GetAllAndSearchManagerDTO>());
                }
                else
                {
                    logger.LogError(ex, "Error happened while Displaying employees in department");
                    return PartialView("Errorpartial", ex);
                }
            }
        }


    }
}


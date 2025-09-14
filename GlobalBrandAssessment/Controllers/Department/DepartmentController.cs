using System.Linq.Expressions;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.PL.ViewModels;
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

        public DepartmentController(IDepartmentService departmentService,IManagerService managerService,ILogger<DepartmentController> logger,IWebHostEnvironment environment)
        {
            this.departmentService = departmentService;
            this.managerService = managerService;
            this.logger = logger;
            this.environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
            try
            {
                var department = await departmentService.GetAllAsync();
                return View(department);
            }

            catch (Exception ex) 
            {
                logger.LogError(ex, "Error happened while Get All department");
                return PartialView("Errorpartial", ex);
            }

        }



        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")

                return RedirectToAction("Index", "Login");

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
        public async Task<IActionResult> Create()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")

                return RedirectToAction("Index", "Login");
            try {
                var managers = await managerService.GetAllManagersAsync();
                ViewBag._Manager = new SelectList(managers, "Id", "FirstName");
                return View();
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
                    return PartialView("Errorpartial", ex);
                }


            }



        }

        [HttpPost]
        public async Task<IActionResult> Create(AddAndUpdateDepartmentDTO department)
        {
           
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }

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
        public async Task< IActionResult> Edit(int? id)
        {
            try {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
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
        public async Task<IActionResult> Edit([FromRoute]int id,AddAndUpdateDepartmentDTO department)
        {
           
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }

            var managers = await managerService.GetAllManagersAsync();
            if (ModelState.IsValid)
                {
                    try {
                    department.Id = id;
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
        public async Task<IActionResult> Delete(int? id)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {               
                    return Json(new { success = false, redirectUrl = Url.Action("Index", "Login") });                
            }
            try
            {

                if (!id.HasValue) {
                    TempData["Message"] = "Invalid department ID.";
                    return Json(new { success = false, redirectUrl = Url.Action("Index", "Department") });
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
                    return Json(new { success = false, redirectUrl = Url.Action("Index", "Department") });
                }
            }

            catch (Exception ex) 
            {
                logger.LogError(ex, "Error happened while delete department");
                TempData["Message"] = "Something went wrong, please try again later.";
                return Json(new { success = false, redirectUrl = Url.Action("Index", "Department") });
               
            }

        }
    }
}

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

        public DepartmentController(IDepartmentService departmentService,IManagerService managerService,ILogger<DepartmentController> logger)
        {
            this.departmentService = departmentService;
            this.managerService = managerService;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }

                var department = await departmentService.GetAllAsync();
                return View(department);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while Display department");
                TempData["Message"] = "Something went wrong, please try again later.";
                return RedirectToAction("Index", "Department");
            }

        }



        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            try {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
                var department = await departmentService.SearchAsync(searchname);
                if (department == null || !department.Any())
                {
                    TempData["Message"] = "No departments found.";
                    return PartialView("_IndexDepartmentPartial", new List<GetAllandSearchDepartmentDTO>());
                }
                return PartialView("_IndexDepartmentPartial", department);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while search department");
                return StatusCode(500, "Server is not work");

            }

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
                var managers = await managerService.GetAllManagersAsync();
                ViewBag._Manager = new SelectList(managers, "Id", "FirstName");
                return View();
            }
               
            
            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while Create department");
                TempData["Message"] = "Something went wrong, please try again later.";
                return RedirectToAction("Index", "Department");
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddAndUpdateDepartmentDTO department)
        {
            try {

                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }

                if (ModelState.IsValid)
                {
                    int result = await departmentService.AddAsync(department);
                    if (result > 0)
                    {
                        TempData["Message"] = "Department created successfully.";
                        return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to create Department.";
                        return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                    }


                }
                var managers = await managerService.GetAllManagersAsync();
                ViewBag._Manager = new SelectList(managers, "Id", "FirstName");
                return PartialView("_CreateDepartmentPartial", department);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while Create department");
                TempData["Message"] = "Something went wrong, please try again later.";
                return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
            }
          
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
                if (id is null || id < 0)
                {
                    return RedirectToAction("Index", "Department");
                }


                var department = await departmentService.GetDepartmentByIdAsync(id);
                if (department == null)
                {
                    TempData["Message"] = "Department not found.";
                    return RedirectToAction("Index");
                }
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
        public async Task<IActionResult> Edit(AddAndUpdateDepartmentDTO department)
        {
            try {


                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }

                if (ModelState.IsValid)
                {

                    int result = await departmentService.UpdateAsync(department);
                    if (result > 0)
                    {
                        TempData["Message"] = "Department updated successfully.";
                        return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to update Department.";
                        return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                    }
                }

                var managers = await managerService.GetAllManagersAsync();
                ViewBag._Manager = new SelectList(managers, "Id", "FirstName", department.ManagerId); ;
                return PartialView("_EditDepartmentPartial", department);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while Edit department");
                TempData["Message"] = "Something went wrong, please try again later.";
              
                return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
                if (id is null)
                {
                    TempData["Message"] = "Department Id is not exist";
                    return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                }

                var result = await departmentService.DeleteAsync(id);
                if (result > 0)
                {
                    TempData["Message"] = "Department delete successfully.";
                    return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                }
                else
                {
                    TempData["Message"] = "You Cant Delete Because Department exist employees";
                    return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                }
            }

            catch (Exception ex) 
            {
                logger.LogError(ex, "Error happened while delete department");
                TempData["Message"] = "Something went wrong, please try again later.";
                return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
               
            }

        }
    }
}

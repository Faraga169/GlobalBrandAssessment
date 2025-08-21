using System.Threading.Tasks;
using GlobalBrandAssessment.BL.Services;
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

        public DepartmentController(IDepartmentService departmentService,IManagerService managerService)
        {
            this.departmentService = departmentService;
            this.managerService = managerService;

        }
        [HttpGet]
        public IActionResult Index()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }

            var department = departmentService.GetAll();
            return View(department);
        }



        [HttpPost]
        public IActionResult Search(string searchname)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            var department = departmentService.Search(searchname).ToList();
            if (department == null ||!department.Any())
            {
                TempData["Message"] = "No departments found.";
                return PartialView("_IndexDepartmentPartial",new List<DAL.Data.Models.Department>());
            }
            return PartialView("_IndexDepartmentPartial",department);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag._Manager = new SelectList(managerService.GetAllManagers(), "Id", "FullName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateDepartmentViewModel department)
        {

            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var existingDepartment = new DAL.Data.Models.Department()
                {
                    Name = department.Name,
                    ManagerId = department.ManagerId
                };
                int result = departmentService.Add(existingDepartment);


                if (result > 0)
                {
                    TempData["Message"] = "Department created successfully.";
                    return Json(new { success = true });
                }
                else
                {
                    TempData["Message"] = "Failed to create Department.";
                    ViewBag._Manager = new SelectList(managerService.GetAllManagers(), "Id", "FullName");
                    return PartialView("_CreateDepartmentPartial",department);
                }


            }
            ViewBag._Manager = new SelectList(managerService.GetAllManagers(), "Id", "FullName");
            return PartialView("_CreateDepartmentPartial", department);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null|| id<0)
            {
                return RedirectToAction("Index", "Department");
            }
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }

            var department = departmentService.GetDepartmentById(id);
            if (department == null)
            {
                TempData["Message"] = "Department not found.";
                return RedirectToAction("Index");
            }
            ViewBag._Manager = new SelectList(managerService.GetAllManagers(), "Id", "FullName", department.ManagerId);
            return View(department);
        }

        [HttpPost]
        public IActionResult Edit(DAL.Data.Models.Department department)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var existingdepartment = departmentService.GetDepartmentById(department.Id);


                if (existingdepartment == null)
                    return RedirectToAction("Edit"); ;
                existingdepartment.Name = department.Name;
                existingdepartment.ManagerId = department.ManagerId;
                int result = departmentService.Update(existingdepartment);
                if (result > 0)
                {
                    TempData["Message"] = "Department updated successfully.";
                    return Json(new { success = true });
                }
                else
                {
                    TempData["Message"] = "Failed to update Department.";
                    return PartialView("_EditDepartmentPartial", department);
                }
            }

            ViewBag._Manager = new SelectList(managerService.GetAllManagers(), "Id", "FullName", department.ManagerId);
            return PartialView("_EditDepartmentPartial", department);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                TempData["Message"] = "Department Id is not exist";
                return Json(new { success = true });
            }
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            var result = departmentService.Delete(id);
            if (result > 0)
            {
                TempData["Message"] = "Department delete successfully.";
                return Json(new { success = true });
            }
            else
            {
                TempData["Message"] = "You Cant Delete Because Department exist employees";
                return Json(new { success = false });
            }
        }
    }
}

using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
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
                return PartialView("_IndexDepartmentPartial",new List<GetAllandSearchDepartmentDTO>());
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
        public IActionResult Create(AddAndUpdateDepartmentDTO department)
        {

            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
           
            if (ModelState.IsValid)
            {
                int result = departmentService.Add(department);
                if (result > 0)
                {
                    TempData["Message"] = "Department created successfully.";
                    return Json(new { success = true ,redirecturl=Url.Action("Index","Department")});
                }
                else
                {
                    TempData["Message"] = "Failed to create Department.";
                    return Json(new { success = false, redirecturl = Url.Action("Index", "Department") });
                }


            }
            ViewBag._Manager = new SelectList(managerService.GetAllManagers(), "Id", "FullName");
            return PartialView("_CreateDepartmentPartial", department);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null|| id<0)
            {
                return RedirectToAction("Index", "Department");
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
        public IActionResult Edit(AddAndUpdateDepartmentDTO department)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {

                int result = departmentService.Update(department);
                if (result > 0)
                {
                    TempData["Message"] = "Department updated successfully.";
                    return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
                }
                else
                {
                    TempData["Message"] = "Failed to update Department.";
                    return Json(new { success = false, redirecturl = Url.Action("Index", "Department") });
                }
            }

            ViewBag._Manager = new SelectList(managerService.GetAllManagers(), "Id", "FullName", department.ManagerId);
            return PartialView("_EditDepartmentPartial", department);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
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
           
            var result = departmentService.Delete(id);
            if (result > 0)
            {
                TempData["Message"] = "Department delete successfully.";
                return Json(new { success = true, redirecturl = Url.Action("Index", "Department") });
            }
            else
            {
                TempData["Message"] = "You Cant Delete Because Department exist employees";
                return Json(new { success = false, redirecturl = Url.Action("Index", "Department") });
            }
        }
    }
}

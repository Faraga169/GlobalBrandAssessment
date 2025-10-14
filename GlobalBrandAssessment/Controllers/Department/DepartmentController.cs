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
using Serilog;

namespace GlobalBrandAssessment.PL.Controllers.Department
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService departmentService;
        private readonly IManagerService managerService;
        private readonly IEmployeeService employeeService;
        private readonly UserManager<User> userManager;

        public DepartmentController(IDepartmentService departmentService, IManagerService managerService, IEmployeeService employeeService, UserManager<User> userManager)
        {
            this.departmentService = departmentService;
            this.managerService = managerService;
            this.employeeService = employeeService;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;
            var departments = new List<GetAllandSearchDepartmentDTO>();

            if (User.IsInRole("Admin"))
            {
                departments = await departmentService.GetAllAsync();
                Log.ForContext("UserName", currentUser?.UserName)
                   .ForContext("ActionType", "Index")
                   .ForContext("Controller", "Department")
                   .Information("Admin viewed all departments");
            }
            else if (User.IsInRole("Manager"))
            {
                departments = await departmentService.GetDepartmentByManagerId(managerId);
                Log.ForContext("UserName", currentUser?.UserName)
             .ForContext("ActionType", "Index")
             .ForContext("Controller", "Department")
             .Information("Manager {UserName} viewed their departments", currentUser?.UserName);
            }

            return View(departments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string searchname)
        {

            var currentUser = await userManager.GetUserAsync(User);
            Log.ForContext("UserName",currentUser?.UserName)
               .ForContext("ActionType", "Search")
               .ForContext("Controller", "Department")
               .Information("Admin searched departments with keyword: {SearchKeyword}", searchname);

            var departments = await departmentService.SearchAsync(searchname);
            if (departments == null || !departments.Any())
            {
                Log.ForContext("UserName", currentUser?.UserName)
                    .ForContext("ActionType", "Search")
                    .ForContext("Controller", "Department")
                    .Warning("No departments found for search: {SearchKeyword}", searchname);
                return PartialView("_IndexDepartmentPartial", new List<GetAllandSearchDepartmentDTO>());
            }

            return PartialView("_IndexDepartmentPartial", departments);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Log.ForContext("UserName", User.Identity?.Name)
               .ForContext("ActionType", "Create")
               .ForContext("Controller", "Department")
               .Information("Admin accessed Create Department page");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddAndUpdateDepartmentDTO department)
        {
            var managers = await managerService.GetAllManagersAsync();
            ModelState.Remove("ManagerId");

            if (ModelState.IsValid)
            {
                int result = await departmentService.AddAsync(department);

                if (result > 0)
                {
                    Log.ForContext("UserName", User.Identity?.Name)
                       .ForContext("ActionType", "Create")
                       .ForContext("Controller", "Department")
                       .Information("Department {DeptName} created successfully by Admin", department.Name);
                    TempData["Message"] = "Department created successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
                else
                {
                    Log.ForContext("UserName", User.Identity?.Name)
                       .ForContext("ActionType", "Create")
                       .ForContext("Controller", "Department")
                       .Warning("Failed to create Department {DeptName}", department.Name);
                    TempData["Message"] = "Failed to create Department.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
            }
            Log.ForContext("UserName", User.Identity?.Name)
                      .ForContext("ActionType", "Create")
                      .ForContext("Controller", "Department")
                      .Warning("Validation failed while creating department {DeptName}", department.Name);
            ViewBag._Manager = new SelectList(managers, "Id", "FirstName");
            return PartialView("_CreateDepartmentPartial", department);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "Edit")
                   .ForContext("Controller", "Department")
                   .Warning("Edit Department called with invalid Id");
                return BadRequest();
            }

            var department = await departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "Edit")
                   .ForContext("Controller", "Department")
                    .Warning("Department with Id {DeptId} not found for edit", id);
                return NotFound();
            }

            var managers = await managerService.GetAllManagersAsync();
            ViewBag._Manager = new SelectList(managers, "Id", "FullName");
            Log.ForContext("UserName", User.Identity?.Name)
               .ForContext("ActionType", "Edit")
               .ForContext("Controller", "Department")
               .Information("Admin opened Edit page for Department: {DeptName}", department.Name);
            return View(department);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit([FromRoute] int id, AddAndUpdateDepartmentDTO department)
        {
            var managers = await managerService.GetAllManagersAsync();

            if (ModelState.IsValid)
            {
                int result = await departmentService.UpdateAsync(department);

                if (result > 0)
                {
                    Log.ForContext("UserName", User.Identity?.Name)
                       .ForContext("ActionType", "Edit")
                       .ForContext("Controller", "Department")
                       .Information("Department {DeptName} updated successfully", department.Name);
                    TempData["Message"] = "Department updated successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
                else
                {
                    Log.ForContext("UserName", User.Identity?.Name)
                       .ForContext("ActionType", "Edit")
                       .ForContext("Controller", "Department")
                       .Warning("Failed to update Department {DeptName}", department.Name);
                    TempData["Message"] = "Failed to update Department.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
            }

            Log.ForContext("UserName", User.Identity?.Name)
                       .ForContext("ActionType", "Edit")
                       .ForContext("Controller", "Department")
                       .Warning("Validation failed while updating Department {DeptName}", department.Name);
            ViewBag._Manager = new SelectList(managers, "Id", "FirstName", department.ManagerId);
            return PartialView("_EditDepartmentPartial", department);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "Delete")
                   .ForContext("Controller", "Department")
                   .Warning("Attempted to delete department with invalid Id");
                TempData["Message"] = "Invalid department ID.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
            }

            var result = await departmentService.DeleteAsync(id);
            if (result > 0)
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "Delete")
                   .ForContext("Controller", "Department")
                   .Information("Department  {DeptId} deleted successfully", id);
                TempData["Message"] = "Department deleted successfully.";
            }
            else
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "Delete")
                   .ForContext("Controller", "Department")
                   .Warning("Failed to delete Department Id {DeptId} has employees", id);
                TempData["Message"] = "You can't delete because department has employees.";
            }

            return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Details(int id)
        {
            var employees = await employeeService.GetEmployeesByDeptId(id);
            Log.ForContext("UserName", User.Identity?.Name)
               .ForContext("ActionType", "Details")
               .ForContext("Controller", "Department")
               .Information("{UserName} viewed employees for Department Id: {DeptId}", User.Identity?.Name, id);
            return View(employees);
        }
    }
}

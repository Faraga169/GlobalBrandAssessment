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

        public DepartmentController(
            IDepartmentService departmentService,
            IManagerService managerService,
            IEmployeeService employeeService,
            UserManager<User> userManager)
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
                   .ForContext("ActionType", "ViewAllDepartments")
                   .ForContext("Controller", "Department")
                   .Information("{UserName} viewed all departments list.", currentUser?.UserName);
            }
            else if (User.IsInRole("Manager"))
            {
                departments = await departmentService.GetDepartmentByManagerId(managerId);
                Log.ForContext("UserName", currentUser?.UserName)
                   .ForContext("ActionType", "ViewManagerDepartments")
                   .ForContext("Controller", "Department")
                   .Information("Manager {UserName} viewed their assigned departments.", currentUser?.UserName);
            }

            return View(departments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string searchname)
        {
            var currentUser = await userManager.GetUserAsync(User);

            Log.ForContext("UserName", currentUser?.UserName)
               .ForContext("ActionType", "SearchDepartments")
               .ForContext("Controller", "Department")
               .Information("{UserName} searched departments using keyword: {SearchKeyword}", currentUser?.UserName, searchname);

            var departments = await departmentService.SearchAsync(searchname);

            if (departments == null || !departments.Any())
            {
                Log.ForContext("UserName", currentUser?.UserName)
                   .ForContext("ActionType", "SearchDepartments")
                   .ForContext("Controller", "Department")
                   .Warning("No departments found for keyword: {SearchKeyword}", searchname);

                return PartialView("_IndexDepartmentPartial", new List<GetAllandSearchDepartmentDTO>());
            }

            return PartialView("_IndexDepartmentPartial", departments);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Log.ForContext("UserName", User.Identity?.Name)
               .ForContext("ActionType", "AccessCreateDepartmentPage")
               .ForContext("Controller", "Department")
               .Information("{UserName} accessed the Create Department page.", User.Identity?.Name);

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
                       .ForContext("ActionType", "CreateDepartment")
                       .ForContext("Controller", "Department")
                       .Information("{UserName} successfully created department: {DeptName}", User.Identity?.Name, department.Name);

                    TempData["Message"] = "Department created successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
                else
                {
                    Log.ForContext("UserName", User.Identity?.Name)
                       .ForContext("ActionType", "CreateDepartment")
                       .ForContext("Controller", "Department")
                       .Warning("{UserName} failed to create department: {DeptName}", User.Identity?.Name, department.Name);

                    TempData["Message"] = "Failed to create Department.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
            }

            Log.ForContext("UserName", User.Identity?.Name)
               .ForContext("ActionType", "ValidationFailed_CreateDepartment")
               .ForContext("Controller", "Department")
               .Warning("Validation failed while creating department: {DeptName}", department.Name);

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
                   .ForContext("ActionType", "InvalidDepartmentEditRequest")
                   .ForContext("Controller", "Department")
                   .Warning("{UserName} attempted to edit with invalid department ID.", User.Identity?.Name);
                return BadRequest();
            }

            var department = await departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "DepartmentNotFound_Edit")
                   .ForContext("Controller", "Department")
                   .Warning("Department with ID {DeptId} not found for edit.", id);
                return NotFound();
            }

            var managers = await managerService.GetAllManagersAsync();
            ViewBag._Manager = new SelectList(managers, "Id", "FullName");

            Log.ForContext("UserName", User.Identity?.Name)
               .ForContext("ActionType", "AccessEditDepartmentPage")
               .ForContext("Controller", "Department")
               .Information("{UserName} opened edit page for department: {DeptName}", User.Identity?.Name, department.Name);

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
                       .ForContext("ActionType", "UpdateDepartment")
                       .ForContext("Controller", "Department")
                       .Information("{UserName} successfully updated department: {DeptName}", User.Identity?.Name, department.Name);

                    TempData["Message"] = "Department updated successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
                else
                {
                    Log.ForContext("UserName", User.Identity?.Name)
                       .ForContext("ActionType", "UpdateDepartmentFailed")
                       .ForContext("Controller", "Department")
                       .Warning("{UserName} failed to update department: {DeptName}", User.Identity?.Name, department.Name);

                    TempData["Message"] = "Failed to update Department.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
                }
            }

            Log.ForContext("UserName", User.Identity?.Name)
               .ForContext("ActionType", "ValidationFailed_UpdateDepartment")
               .ForContext("Controller", "Department")
               .Warning("Validation failed while updating department: {DeptName}", department.Name);

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
                   .ForContext("ActionType", "InvalidDeleteRequest")
                   .ForContext("Controller", "Department")
                   .Warning("{UserName} attempted to delete a department with invalid ID.", User.Identity?.Name);

                TempData["Message"] = "Invalid department ID.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Department") });
            }
            var department=await departmentService.GetDepartmentByIdAsync(id);
            var result = await departmentService.DeleteAsync(id);
            if (result > 0)
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "DeleteDepartment")
                   .ForContext("Controller", "Department")
                   .Information("{UserName} deleted department {DeptId}", User.Identity?.Name, department?.Name);
                TempData["Message"] = "Department deleted successfully.";
            }
            else
            {
                Log.ForContext("UserName", User.Identity?.Name)
                   .ForContext("ActionType", "DeleteDepartmentFailed_HasEmployees")
                   .ForContext("Controller", "Department")
                   .Warning("Admin {UserName} failed to delete department ID {DeptId} because it has assigned employees.", User.Identity?.Name, id);
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
               .ForContext("ActionType", "ViewDepartmentDetails")
               .ForContext("Controller", "Department")
               .Information("{UserName} viewed employees belonging to department ID: {DeptId}", User.Identity?.Name, id);

            return View(employees);
        }
    }
}

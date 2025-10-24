using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Data.Models.Common;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.PL.Controllers.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GlobalBrandAssessment.PL.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IManagerService managerService;
        private readonly UserManager<User> userManager;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IMapper mapper;
        private readonly ITaskService taskService;

        public AdminController(IManagerService managerService, UserManager<User> userManager, IEmployeeService employeeService, IDepartmentService departmentService, IMapper mapper, ITaskService taskService)
        {
            this.managerService = managerService;
            this.userManager = userManager;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.mapper = mapper;
            this.taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchname, int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName")
        {
            PagedResult<GetAllAndSearchManagerDTO> result;

            if (!string.IsNullOrEmpty(searchname))
            {
                result = await managerService.SearchAsync(searchname, null, pageno, pagesize, sortcolumn);

                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("Controller", "AdminController")
                   .ForContext("ActionType", "SearchEmployee")
                   .Information(" {UserName} searched employees with keyword '{SearchKeyword}' on page {PageNumber}",
                                User?.Identity?.Name, searchname, pageno);
            }
            else
            {
                result = await employeeService.GetAllPagedAsync(pageno, pagesize, sortcolumn);

                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("Controller", "AdminController")
                   .ForContext("ActionType", "ViewEmployeeList")
                   .Information("{UserName} viewed employee list on page {PageNumber}",
                                User?.Identity?.Name, pageno);
            }

            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await departmentService.GetAllAsync();
            ViewBag._Department = new SelectList(departments, "Id", "Name");
            var Roles = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(r => new SelectListItem
                {
                    Value = r.ToString(),
                    Text = r.ToString()
                }).ToList();
            ViewBag.Roles = new SelectList(Roles, "Value", "Text");
            Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("Controller", "AdminController")
              .ForContext("ActionType", "CreateEmployeeView")
              .Information("{UserName} opened Create page to Create Employee",
                           User?.Identity?.Name);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
            var department = await departmentService.GetDepartmentByIdAsync(addORUpdateManagerDTO.DeptId);

            if (addORUpdateManagerDTO.Role == "Employee")
            {
                var manager = await managerService.GetManagerByDepartmentIdAsync(addORUpdateManagerDTO.DeptId);
                addORUpdateManagerDTO.ManagerId = manager?.Id;
            }

            if (addORUpdateManagerDTO.Role == "Manager" && department != null && department.ManagerId != null)
            {
                ModelState.AddModelError("", "This Department Already Has a Manager assigned");
            }

            if (ModelState.IsValid)
            {
                if (addORUpdateManagerDTO.Image != null)
                {
                    string rootPath = Directory.GetCurrentDirectory();
                    string wwwRootPath = Path.Combine(rootPath, "wwwroot");
                    string fileName = Path.GetFileNameWithoutExtension(addORUpdateManagerDTO.Image.FileName);
                    string extension = Path.GetExtension(addORUpdateManagerDTO.Image.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await addORUpdateManagerDTO.Image.CopyToAsync(fileStream);
                    }
                    addORUpdateManagerDTO.ImageURL = "/images/" + fileName;
                }

                int newEmployeeId = await managerService.Add(addORUpdateManagerDTO);

                if (newEmployeeId > 0)
                {
                    var user = mapper.Map<AddAndUpdateManagerDTO, User>(addORUpdateManagerDTO);
                    user.EmployeeId = newEmployeeId;
                    user.Email = $"{user.UserName}@test.com";
                    user.EmailConfirmed = true;
                    user.Id = Guid.NewGuid().ToString();
                    var result = await userManager.CreateAsync(user, addORUpdateManagerDTO.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, addORUpdateManagerDTO.Role);

                        if (addORUpdateManagerDTO.Role == "Manager" && addORUpdateManagerDTO.DeptId.HasValue)
                        {
                            if (department != null && department.ManagerId == null)
                            {
                                department.ManagerId = newEmployeeId;
                                await departmentService.UpdateAsync(department);
                                await managerService.DemoteManagerToEmployeeAsync(department.ManagerId);
                            }
                        }

                        Log.ForContext("UserName", User?.Identity?.Name)
                           .ForContext("Controller", "AdminController")
                           .ForContext("ActionType", "CreateEmployee")
                           .Information("{UserName} created employee {EmployeeName} with Role {Role} in Department {DeptName}",
                                        User?.Identity?.Name, addORUpdateManagerDTO.FirstName, addORUpdateManagerDTO.Role, department?.Name);
                    }
                    else
                    {
                        Log.ForContext("UserName", User?.Identity?.Name)
                           .ForContext("Controller", "AdminController")
                           .ForContext("ActionType", "CreateEmployee")
                           .Warning("{UserName} failed to create employee {EmployeeName}", User?.Identity?.Name, addORUpdateManagerDTO.FirstName);
                    }

                    TempData["Message"] = "Employee created successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                }
                else
                {
                    TempData["Message"] = "Failed to create employee.";
                    Log.ForContext("UserName", User?.Identity?.Name)
                       .ForContext("Controller", "AdminController")
                       .ForContext("ActionType", "CreateEmployee")
                       .Warning("{UserName} failed to create employee {EmployeeName}", User?.Identity?.Name, addORUpdateManagerDTO.FirstName);

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                }
            }

            var departments = await departmentService.GetAllAsync();
            ViewBag._Department = new SelectList(departments, "Id", "Name");
            var Roles = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(r => new SelectListItem { Value = r.ToString(), Text = r.ToString() })
                .ToList();
            ViewBag.Roles = new SelectList(Roles, "Value", "Text");
            return PartialView("_CreateAdminPartial", addORUpdateManagerDTO);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var employee = await employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            var result = mapper.Map<DAL.Data.Models.Employee, AddAndUpdateManagerDTO>(employee);
            var departments = await departmentService.GetAllAsync();
            ViewBag._Department = new SelectList(departments, "Id", "Name", employee.DeptId);
            var Roles = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(r => new SelectListItem { Value = r.ToString(), Text = r.ToString() })
                .ToList();
            ViewBag.Roles = new SelectList(Roles, "Value", "Text", result.Role);

            Log.ForContext("UserName", User?.Identity?.Name)
               .ForContext("Controller", "AdminController")
               .ForContext("ActionType", "EditEmployeeView")
               .Information("{UserName} opened edit page for employee {EmployeeName}",
                            User?.Identity?.Name, result.FirstName);

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
            ModelState.Remove("Password");
            var department = await departmentService.GetDepartmentByIdAsync(addORUpdateManagerDTO.DeptId);
            var manager = await managerService.GetManagerByDepartmentIdAsync(addORUpdateManagerDTO.DeptId);
            var oldImageUrl = await employeeService.GetEmployeeImageUrlAsync(addORUpdateManagerDTO.Id);

            if (ModelState.IsValid)
            {
                if (addORUpdateManagerDTO.Image != null)
                {
                    string rootPath = Directory.GetCurrentDirectory();
                    string wwwRootPath = Path.Combine(rootPath, "wwwroot");
                    string fileName = Path.GetFileNameWithoutExtension(addORUpdateManagerDTO.Image.FileName);
                    string extension = Path.GetExtension(addORUpdateManagerDTO.Image.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        addORUpdateManagerDTO.Image.CopyTo(fileStream);
                    }
                    addORUpdateManagerDTO.ImageURL = "/images/" + fileName;

                    if (!string.IsNullOrEmpty(oldImageUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, oldImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }
                else
                {
                    addORUpdateManagerDTO.ImageURL = oldImageUrl;
                }

                int result = await managerService.UpdateAsync(addORUpdateManagerDTO);

                if (result > 0)
                {
                    var user = await userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == addORUpdateManagerDTO.Id);
                    if (user != null)
                    {
                        user.UserName = addORUpdateManagerDTO.FirstName;
                        user.Email = $"{addORUpdateManagerDTO.FirstName}@test.com";
                        user.EmailConfirmed = true;
                        await userManager.UpdateAsync(user);
                        var currentRoles = await userManager.GetRolesAsync(user);
                        if (currentRoles.Any()) await userManager.RemoveFromRolesAsync(user, currentRoles);
                        await userManager.AddToRoleAsync(user, addORUpdateManagerDTO.Role);
                    }

                    Log.ForContext("UserName", User?.Identity?.Name)
                       .ForContext("Controller", "AdminController")
                       .ForContext("ActionType", "EditEmployee")
                       .Information("{UserName} updated employee {EmployeeName} with Role {Role}",
                                    User?.Identity?.Name, addORUpdateManagerDTO.FirstName, addORUpdateManagerDTO.Role);

                    TempData["Message"] = "Employee updated successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                }
                else
                {
                    Log.ForContext("UserName", User?.Identity?.Name)
                       .ForContext("Controller", "AdminController")
                       .ForContext("ActionType", "EditEmployee")
                       .Warning("{UserName} failed to update employee {EmployeeName}",
                                User?.Identity?.Name, addORUpdateManagerDTO.FirstName);

                    TempData["Message"] = "Failed to update employee.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                }
            }

            var departmentsList = await departmentService.GetAllAsync();
            ViewBag._Department = new SelectList(departmentsList, "Id", "Name", addORUpdateManagerDTO.DeptId);
            var RolesList = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(r => new SelectListItem { Value = r.ToString(), Text = r.ToString() })
                .ToList();
            ViewBag.Roles = new SelectList(RolesList, "Value", "Text", addORUpdateManagerDTO.Role);

            return PartialView("_EditAdminPartial", addORUpdateManagerDTO);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var result = await managerService.DeleteAsync(id);
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == id);

            if (result > 0)
            {
                if (user != null) await userManager.DeleteAsync(user);

                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("Controller", "AdminController")
                   .ForContext("ActionType", "DeleteEmployee")
                   .Information("{UserName} deleted employee {EmployeeName}",
                                User?.Identity?.Name, user?.Employee?.FirstName, id);

                TempData["Message"] = "Employee deleted successfully.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
            }
            else
            {
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("Controller", "AdminController")
                   .ForContext("ActionType", "DeleteEmployee")
                   .Warning("{UserName} failed to delete employee {EmployeeName}",
                            User?.Identity?.Name, user?.Employee?.FirstName, id);

                if (result == -1)
                    TempData["Message"] = "Can't delete this employee because he has employees under him.";
                else if (result == -2)
                    TempData["Message"] = "Can't delete this employee because he is assigned as a manager of a department.";
                else
                    TempData["Message"] = "Employee delete failed.";

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
            }
        }

    }
}

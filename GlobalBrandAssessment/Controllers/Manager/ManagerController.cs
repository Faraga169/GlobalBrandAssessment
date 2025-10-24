using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.BL.Services;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GlobalBrandAssessment.DAL.Data.Models.Common;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Serilog;

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

        public ManagerController(
            IManagerService managerService,
            UserManager<User> userManager,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IMapper mapper,
            ILogger<ManagerController> logger)
        {
            this.managerService = managerService;
            this.userManager = userManager;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchname, int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName")
        {
            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;

            PagedResult<GetAllAndSearchManagerDTO> result;

            if (!string.IsNullOrEmpty(searchname))
            {
                result = await managerService.SearchAsync(searchname, managerId, pageno, pagesize, sortcolumn);
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("ActionType", "SearchEmployee")
                   .ForContext("Controller", "ManagerController")
                   .Information("Search for {SearchTerm} by {UserName}", searchname, currentUser?.UserName);
            }
            else
            {
                result = await employeeService.GetEmployeesByManagerPagedAsync(managerId, pageno, pagesize, sortcolumn);
                Log.ForContext("UserName", currentUser?.UserName)
                   .ForContext("ActionType", "ViewEmployeeList")
                   .ForContext("Controller", "ManagerController")
                   .Information("Manager {ManagerName} viewed employee list ", currentUser?.UserName);
            }
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var employee = await employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("ActionType", "ViewEmployeeDetails")
                   .ForContext("Controller", "ManagerController")
                   .Warning("Manager {UserName} tried to view non-existent employee with ID {EmployeeId}", User?.Identity?.Name, id);
                return NotFound();
            }

            Log.ForContext("UserName", User?.Identity?.Name)
                .ForContext("ActionType", "ViewEmployeeDetails")
                .ForContext("Controller", "ManagerController")
                .Information("Manager {UserName} viewed details for employee {EmployeeName}", User?.Identity?.Name, employee.FirstName);
            return View(employee);
        }

    }
}

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
        public async Task<IActionResult> Index(int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName")
        {
            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;

            var manager = await employeeService.GetEmployeesByManagerPagedAsync(managerId, pageno, pagesize, sortcolumn);

            Log.ForContext("UserName", currentUser.UserName)
                .ForContext("ActionType", "Index")
                .ForContext("Controller", "Manager")
                .Information("Manager {ManagerName} viewed employee list ", currentUser.UserName);

            return View(manager);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            if (string.IsNullOrWhiteSpace(searchname))
            {
                Log.ForContext("UserName", User?.Identity?.Name)
               .ForContext("ActionType", "Search")
               .ForContext("Controller", "Manager")
               .Warning("Empty search term provided by user {UserName}", User.Identity?.Name);
                return PartialView("_IndexManagerPartial", new List<GetAllAndSearchManagerDTO>());
            }

            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;

            List<GetAllAndSearchManagerDTO> managerResults;

            if (User.IsInRole("Manager"))
                managerResults = await managerService.SearchAsync(searchname, managerId);
            else
                managerResults = await managerService.SearchAsync(searchname, null);

         
            if (managerResults == null || !managerResults.Any())
                Log.ForContext("UserName", User?.Identity?.Name)
               .ForContext("ActionType", "Search")
               .ForContext("Controller", "Manager")
                .Warning("Search for '{SearchTerm}' by {UserName} returned no results", searchname, User.Identity?.Name);
            else
                Log.ForContext("UserName", User?.Identity?.Name)
               .ForContext("ActionType", "Search")
               .ForContext("Controller", "Manager")
                .Information("Search for {SearchTerm} by {UserName}", searchname, User.Identity?.Name);

            return PartialView("_IndexManagerPartial", managerResults ?? new List<GetAllAndSearchManagerDTO>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var employee = await employeeService.GetEmployeeByIdAsync(id);

           
            if (employee == null)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
               .ForContext("ActionType", "Details")
               .ForContext("Controller", "Manager")
               .Warning("Manager {UserName} tried to view non-existent employee with ID {EmployeeId}", User.Identity?.Name, id);
                return NotFound();
            }

            Log.ForContext("UserName", User?.Identity?.Name)
                .ForContext("ActionType", "Details")
                .ForContext("Controller", "Manager")
                .Information("Manager {UserName} viewed details for employee {EmployeeName}", User.Identity?.Name, employee.FirstName);
            return View(employee);
        }
    }
}

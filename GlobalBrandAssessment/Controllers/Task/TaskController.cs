using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System;

namespace GlobalBrandAssessment.PL.Controllers.Task
{
    [Authorize(Roles = "Manager,Admin")]
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly UserManager<User> userManager;
        private readonly IAttachmentService attachmentService;
        private readonly ICommentService commentService;
        private readonly IEmployeeService employeeService;
        private readonly ILogger<TaskController> logger;

        public TaskController(
            ITaskService taskService,
            UserManager<User> userManager,
            IAttachmentService attachmentService,
            ICommentService commentService,
            IEmployeeService employeeService,
            ILogger<TaskController> logger)
        {
            this.taskService = taskService;
            this.userManager = userManager;
            this.attachmentService = attachmentService;
            this.commentService = commentService;
            this.employeeService = employeeService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = new List<GetAllandSearchTaskDTO>();

            if (User.IsInRole("Manager"))
            {
                var currentUser = await userManager.GetUserAsync(User);
                var employeeId = currentUser?.EmployeeId;
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("ActionType", "ViewAssignedTasks")
                   .ForContext("Controller", "TaskManagement")
                   .Information("Manager {Username} viewed tasks assigned to their employees.", currentUser.UserName);

                tasks = await taskService.GetAllTasksbyManagerIdAsync(employeeId);
            }
            else if (User.IsInRole("Admin"))
            {
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("ActionType", "ViewAllTasks")
                   .ForContext("Controller", "TaskManagement")
                   .Information("Admin viewed all tasks in the system.");
                tasks = await taskService.GetAll();
            }
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;
            Log.ForContext("UserName", User?.Identity?.Name)
               .ForContext("ActionType", "SearchTasks")
               .ForContext("Controller", "TaskManagement")
               .Information("{UserName} searched tasks with keyword: {SearchName}",
                 currentUser?.UserName, searchname);

            var tasks = User.IsInRole("Manager")
                ? await taskService.SearchAsync(searchname, managerId)
                : await taskService.SearchAsync(searchname, null);

            if (tasks == null || !tasks.Any())
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("ActionType", "SearchTasks")
                   .ForContext("Controller", "TaskManagement")
                   .Warning("No search results found for: {SearchName}", searchname);

            return PartialView("_IndexTaskPartial", tasks ?? new List<GetAllandSearchTaskDTO>());
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await userManager.GetUserAsync(User);

            var managerId = currentUser?.EmployeeId;
            if (User.IsInRole("Manager"))
            {
                var employees = await employeeService.GetEmployeesByManagerId(managerId);
                if (employees == null || !employees.Any())
                {
                    ViewBag._Employees = new List<SelectListItem>{
        new SelectListItem { Value = "", Text = "No employees available" }
    };
                }
                else
                {
                    ViewBag._Employees = new SelectList(employees, "Id", "FirstName");
                }
            }

            else if (User.IsInRole("Admin"))
            {
                var employees = await employeeService.GetAll();
                if (employees == null || !employees.Any())
                {
                    ViewBag._Employees = new List<SelectListItem>{
        new SelectListItem { Value = "", Text = "No employees available" }
    };
                }
                else
                {
                    ViewBag._Employees = new SelectList(employees, "Id", "FirstName");
                }
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(AddandUpdateTaskDTO createtaskdto)
        {
            if (!ModelState.IsValid)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("ActionType", "CreateTask")
                   .ForContext("Controller", "TaskManagement")
                   .Warning("Invalid data while creating a task.");
                return PartialView("_CreateTaskPartial", createtaskdto);
            }

            int result = await taskService.AddAsync(createtaskdto);

            if (result > 0)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
                   .ForContext("ActionType", "CreateTask")
                   .ForContext("Controller", "TaskManagement")
                   .Information("Task '{Title}' created successfully by {UserName}.", createtaskdto.Title, User?.Identity?.Name);
                TempData["Message"] = "Task created successfully.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
            }

            Log.ForContext("UserName", User?.Identity?.Name)
               .ForContext("ActionType", "CreateTask")
               .ForContext("Controller", "TaskManagement")
               .Warning("Failed to create task: {Title}", createtaskdto.Title);
            TempData["Message"] = "Failed to create task.";
            return Json(new { success = false });
        }

    }
}

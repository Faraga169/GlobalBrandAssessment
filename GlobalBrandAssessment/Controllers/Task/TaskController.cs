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
               .ForContext("ActionType", "Index")
               .ForContext("Controller", "Task")
               .Information("Manager {Username} is viewing tasks.", currentUser.UserName);

                tasks = await taskService.GetAllTasksbyManagerIdAsync(employeeId);
            }
            else if (User.IsInRole("Admin"))
            {
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Task")
               .Information("Admin is viewing all tasks.");
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
              .ForContext("ActionType", "Search")
              .ForContext("Controller", "Task")
              .Information("Search requested by {UserName} with keyword: {SearchName}",
                currentUser.UserName, searchname);

            var tasks = User.IsInRole("Manager")
                ? await taskService.SearchAsync(searchname, managerId)
                : await taskService.SearchAsync(searchname, null);

            if (tasks == null || !tasks.Any())
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Search")
              .ForContext("Controller", "Task")
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
              .ForContext("ActionType", "Create")
              .ForContext("Controller", "Task")
               .Warning("Invalid model state during task creation.");
                return PartialView("_CreateTaskPartial", createtaskdto);
            }

            int result = await taskService.AddAsync(createtaskdto);

            if (result > 0)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Create")
              .ForContext("Controller", "Task")
               .Information("Task created successfully: {Title}", createtaskdto.Title);
                TempData["Message"] = "Task created successfully.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
            }
            Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Create")
              .ForContext("Controller", "Task")
            .Warning("Failed to create task: {Title}", createtaskdto.Title);
            TempData["Message"] = "Failed to create task.";
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, AddandUpdateTaskDTO Edittaskdto)
        {
            if (!ModelState.IsValid)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Edit")
              .ForContext("Controller", "Task")
               .Warning("Invalid model state during edit for Task {Title}", Edittaskdto.Title);
                return PartialView("_EditTaskPartial", Edittaskdto);
            }

            int result = await taskService.UpdateAsync(Edittaskdto);

            if (result > 0)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Edit")
              .ForContext("Controller", "Task")
               .Information("Task updated successfully");
                TempData["Message"] = "Task updated successfully.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
            }
            Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Edit")
              .ForContext("Controller", "Task")
            .Warning("Failed to update task {Title}", Edittaskdto.Title);
            TempData["Message"] = "Failed to update task.";
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Delete")
              .ForContext("Controller", "Task")
              .Warning("Delete request with null TaskId.");
                return BadRequest();
            }

            int result = await taskService.DeleteAsync(id);
            if (result > 0)
            {
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Delete")
              .ForContext("Controller", "Task")
              .Information("Task deleted successfully by {UserName}", User?.Identity?.Name);
                TempData["Message"] = "Task deleted successfully.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
            }

            Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Delete")
              .ForContext("Controller", "Task")
            .Warning("Failed to delete task by {UserName}", User?.Identity?.Name);
            TempData["Message"] = "Task deletion failed.";
            return Json(new { success = false });
        }
    }
}

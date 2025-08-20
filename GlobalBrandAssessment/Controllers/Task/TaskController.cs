using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlobalBrandAssessment.PL.Controllers.Task
{
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IManagerService managerService;
        private readonly IEmployeeService employeeService;

        public TaskController(ITaskService taskService,IEmployeeService employeeService)
        {
            this.taskService = taskService;
            this.employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var managerId = HttpContext.Session.GetInt32("UserId");
            if (managerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var tasks = taskService.GetAllTasks(managerId);
            return View(tasks);
        }

        [HttpPost]
        public IActionResult Search(string searchname)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var task = taskService.Search(searchname,mangerId).ToList();
            if (task == null||!task.Any()) 
            {
                TempData["Message"] = "No tasks found.";
                return PartialView("_IndexTaskPartial",new List<Tasks>());
            }
            return PartialView("_IndexTaskPartial", task);
        }


        [HttpGet]
        public IActionResult Create()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag._Employees = new SelectList(employeeService.GetEmployeesByManager(mangerId), "Id", "FullName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateTaskViewModel taskCreateViewModel)
        {

            int? mangerId = HttpContext.Session.GetInt32("UserId");
        
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var tasks = new Tasks()
            {
                Id = taskCreateViewModel.Id,
                Title = taskCreateViewModel.Title,
                Description = taskCreateViewModel.Description,
                EmployeeId = taskCreateViewModel.EmployeeId

            };
            if (ModelState.IsValid)
            {
               

                int result = taskService.Add(tasks);


                if (result > 0)
                {
                    TempData["Message"] = "Task created successfully.";
                    return Json(new { success =true});
                }
                else
                {
                    TempData["Message"] = "Failed to create Task.";
                    ViewBag._Employees = new SelectList(employeeService.GetEmployeesByManager(mangerId), "Id", "FullName");
                    return PartialView("_CreateTaskPartial",taskCreateViewModel);
                }


            }
            ViewBag._Employees = new SelectList(employeeService.GetEmployeesByManager(mangerId), "Id", "FullName");
            return PartialView("_CreateTaskPartial", taskCreateViewModel);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null || id<0)
            {
                return RedirectToAction("Index", "Task");
            }
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var task = taskService.GetTaskById(id.Value);
            if (task == null)
            {
                TempData["Message"] = "Task not found.";
                return RedirectToAction("Index");
            }
            ViewBag._Employees = new SelectList(employeeService.GetEmployeesByManager(mangerId), "Id", "FullName",task.EmployeeId);
            return View(task);
        }

        [HttpPost]
        public IActionResult Edit(CreateTaskViewModel taskCreateViewModel)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var tasks = new Tasks()
            {
                Id = taskCreateViewModel.Id,
                Title = taskCreateViewModel.Title,
                Description = taskCreateViewModel.Description,
                EmployeeId = taskCreateViewModel.EmployeeId

            };
            if (ModelState.IsValid)
            {
               
                int result = taskService.Update(tasks);
                if (result > 0)
                {
                    TempData["Message"] = "Task updated successfully.";
                    return Json(new { success = true });
                }
                else
                {
                    TempData["Message"] = "Failed to update Task.";
                    return PartialView("_EditTaskPartial", taskCreateViewModel);
                }
            }

            ViewBag._Employees = new SelectList(employeeService.GetEmployeesByManager(mangerId), "Id", "FullName");
            return PartialView("_EditTaskPartial", taskCreateViewModel);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                TempData["Message"] = "Task not found.";
                return PartialView("_IndexTaskPartial");
            }
            var manager = HttpContext.Session.GetInt32("UserId");
            if (manager is null)
            {
                return RedirectToAction("Index", "Login");
            }
            var result = taskService.Delete(id);
            if (result > 0)
            {
                TempData["Message"] = "Task delete successfully.";
                return Json(new { success = true });
            }
            else
            {
                TempData["Message"] = "Task delete fail.";
                return PartialView("_IndexTaskPartial");
            }
        }
    }
}

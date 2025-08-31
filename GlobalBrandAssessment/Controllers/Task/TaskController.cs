using GlobalBrandAssessment.BL.DTOS.TaskDTO;
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
        public async Task<IActionResult> Index()
        {
            var managerId = HttpContext.Session.GetInt32("UserId");
            if (managerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var tasks = await taskService.GetAllTasksAsync(managerId);
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var task =await taskService.SearchAsync(searchname,mangerId);
            if (task == null||!task.Any()) 
            {
                TempData["Message"] = "No tasks found.";
                return PartialView("_IndexTaskPartial",new List<GetAllandSearchTaskDTO>());
            }
            return PartialView("_IndexTaskPartial", task);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var employees = await employeeService.GetEmployeesByManagerAsync(mangerId);
            ViewBag._Employees = new SelectList(employees, "Id", "FirstName");
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Create(AddandUpdateTaskDTO createtaskdto)
        {

            int? mangerId = HttpContext.Session.GetInt32("UserId");
        
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            

            if (ModelState.IsValid)
            {

                int result = await taskService.AddAsync(createtaskdto);

                if (result > 0)
                {
                    TempData["Message"] = "Task created successfully.";
                    return Json(new { success =true,redirecturl=Url.Action("Index","Task")});
                }
                else
                {
                    TempData["Message"] = "Failed to create Task.";
                    return Json(new { success = false, redirecturl = Url.Action("Index", "Task") });
                }


            }
            var employee =  await employeeService.GetEmployeesByManagerAsync(mangerId);
            ViewBag._Employees = new SelectList(employee, "Id", "FirstName");
            return PartialView("_CreateTaskPartial", createtaskdto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null || id<0)
            {
                return RedirectToAction("Index", "Task");
            }
          
            var task =await taskService.GetTaskByIdAsync(id.Value);
            if (task == null)
            {
                TempData["Message"] = "Task not found.";
                return RedirectToAction("Index");
            }
            var employee = await employeeService.GetEmployeesByManagerAsync(mangerId);
            ViewBag._Employees = new SelectList(employee, "Id", "FirstName", task.EmployeeId);
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddandUpdateTaskDTO Edittaskdto)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            if (ModelState.IsValid)
            {
               
                int result = await taskService.UpdateAsync(Edittaskdto);
                if (result > 0)
                {
                    TempData["Message"] = "Task updated successfully.";
                    return Json(new { success = true, redirecturl = Url.Action("Index", "Task") });
                }
                else
                {
                    TempData["Message"] = "Failed to update Task.";
                    return Json(new { success = false, redirecturl = Url.Action("Index", "Task") });
                }
            }
            var employee = await employeeService.GetEmployeesByManagerAsync(mangerId);
            ViewBag._Employees = new SelectList(employee, "Id", "FirstName");
            return PartialView("_EditTaskPartial", Edittaskdto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var manager = HttpContext.Session.GetInt32("UserId");
            if (manager is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null)
            {
                TempData["Message"] = "Task not found.";
                return PartialView("_IndexTaskPartial");
            }
          
            var result =await taskService.DeleteAsync(id);
            if (result > 0)
            {
                TempData["Message"] = "Task delete successfully.";
                return Json(new { success = true, redirecturl = Url.Action("Index", "Task") });
            }
            else
            {
                TempData["Message"] = "Task delete fail.";
                return Json(new { success = false, redirecturl = Url.Action("Index", "Task") });
            }
        }
    }
}

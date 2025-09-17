using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlobalBrandAssessment.PL.Controllers.Task
{
    [Authorize(Roles = "Manager")]
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly UserManager<User> userManager;
        private readonly IAttachmentService attachmentService;
        private readonly ICommentService commentService;
        private readonly IEmployeeService employeeService;
        private readonly ILogger<TaskController> logger;
        private readonly IWebHostEnvironment environment;

        public TaskController(ITaskService taskService,UserManager<User> userManager,IAttachmentService attachmentService,ICommentService commentService,IEmployeeService employeeService,ILogger<TaskController> logger,IWebHostEnvironment environment)
        {
            this.taskService = taskService;
            this.userManager = userManager;
            this.attachmentService = attachmentService;
            this.commentService = commentService;
            this.employeeService = employeeService;
            this.logger = logger;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);

            var employeeId = currentUser?.EmployeeId;

            try {
                var tasks = await taskService.GetAllTasksAsync(employeeId);
                return View(tasks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Task Index action.");
                return PartialView("Errorpartial",ex);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            var currentUser = await userManager.GetUserAsync(User);

            var managerId = currentUser?.EmployeeId;

            try
            {
               
                var task = await taskService.SearchAsync(searchname, managerId);
                if (task == null || !task.Any())
                {
                    TempData["Message"] = "No tasks found.";
                    return PartialView("_IndexTaskPartial", new List<GetAllandSearchTaskDTO>());
                }
                return PartialView("_IndexTaskPartial", task);
            }

            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    TempData["Message"] = ex.Message;
                    return PartialView("_IndexTaskPartial", new List<GetAllandSearchTaskDTO>());
                }
                else
                {
                    //2. Deployment => Log Error In File | Table in Database And Return Error View
                    logger.LogError(ex.Message);
                    return PartialView("Errorpartial",ex);
                }


            }

        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await userManager.GetUserAsync(User);

            var managerId = currentUser?.EmployeeId;
            try
            {

                var employees = await employeeService.GetEmployeesByManagerPagedAsync(managerId);
                ViewBag._Employees = new SelectList(employees.Items, "Id", "FirstName");
                return View();
            }
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    ModelState.AddModelError(string.Empty, ex.Message);
                     return View(); 
                }
                else
                {
                    //2. Deployment => Log Error In File | Table in Database And Return Error View
                    logger.LogError(ex.Message);
                    return PartialView("ErrorPartial",ex);
                }


            }
        }

        [HttpPost]
        public async  Task<IActionResult> Create(AddandUpdateTaskDTO createtaskdto)
        {
            


            if (ModelState.IsValid)
            {
                try
                {

                    int result = await taskService.AddAsync(createtaskdto);

                    if (result > 0)
                    {
                        TempData["Message"] = "Task created successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to create Task.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
                    }

                }
                catch (Exception ex)
                {
                    if (environment.IsDevelopment())
                    {
                        // 1.Development => Log Error In Console and Return Same view with Error Message
                        ModelState.AddModelError(string.Empty, ex.Message);


                    }

                    else
                    {

                        //2. Deployment => Log Error In File | Table in Database And Return Error View
                        logger.LogError(ex.Message);
                        return PartialView("Errorpartial", ex);
                    }

                }
            }
            var currentUser = await userManager.GetUserAsync(User);

            var managerId = currentUser?.EmployeeId;
            var employee = await employeeService.GetEmployeesByManagerPagedAsync(managerId);
                ViewBag._Employees = new SelectList(employee.Items, "Id", "FirstName");
                return PartialView("_CreateTaskPartial", createtaskdto);
            }
           

           
        

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUser = await userManager.GetUserAsync(User);

            var managerId = currentUser?.EmployeeId;
            try {

                if (!id.HasValue)
                    return BadRequest();

                var task = await taskService.GetTaskByIdAsync(id.Value);

                if (task == null)
                    return NotFound();
                var employee = await employeeService.GetEmployeesByManagerPagedAsync(managerId);
                ViewBag._Employees = new SelectList(employee.Items, "Id", "FirstName", task.EmployeeId);
                return View(task);

            }
            catch (Exception ex) { 
            logger.LogError(ex, "An error occurred in Task Edit GET action.");
                return StatusCode(500, "Internal server error");
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute]int id,AddandUpdateTaskDTO Edittaskdto)
        {

            Edittaskdto.Id = id;
            if (ModelState.IsValid)
            {

                try
                {

                    int result = await taskService.UpdateAsync(Edittaskdto);
                    if (result > 0)
                    {
                        TempData["Message"] = "Task updated successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to update Task.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
                    }
                }
                catch (Exception ex)
                {
                    if (environment.IsDevelopment())
                    {
                        // 1.Development => Log Error In Console and Return Same view with Error Message
                        ModelState.AddModelError(string.Empty, ex.Message);


                    }
                    else
                    {
                        //2. Deployment => Log Error In File | Table in Database And Return Error View
                        logger.LogError(ex, "Error happened while updating Task {EmployeeId}", Edittaskdto.Id);
                        return PartialView("Errorpartial", ex);
                    }

                }
            }
            var currentUser = await userManager.GetUserAsync(User);

            var managerId = currentUser?.EmployeeId;
            var employee = await employeeService.GetEmployeesByManagerPagedAsync(managerId);
                ViewBag._Employees = new SelectList(employee.Items, "Id", "FirstName");
                return PartialView("_EditTaskPartial", Edittaskdto);
            }
          
           
        

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            
            try {

              
             if(!id.HasValue)
                    return BadRequest();

                await attachmentService.DeleteAsync(id.Value);
                await commentService.DeleteAsync(id.Value);
                var result = await taskService.DeleteAsync(id);
                if (result > 0)
                {
                   
                    TempData["Message"] = "Task delete successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
                }
                else
                {
                    TempData["Message"] = "Task delete fail.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
                }
            }
            catch (Exception ex)
            { 
            logger.LogError(ex, "An error occurred in Task Delete action.");
                    TempData["Message"] = "Task delete fail.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Task") });
            }
                
        }
    }
}

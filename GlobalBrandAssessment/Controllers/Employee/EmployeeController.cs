using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlobalBrandAssessment.PL.Controllers.Employee
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly ITaskService taskService;
        private readonly ICommentService commentService;
        private readonly IAttachmentService attachmentService;
        private readonly IUserService userService;

        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService, ITaskService taskService, ICommentService commentService, IAttachmentService attachmentService,IUserService userService)
        {
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.taskService = taskService;
            this.commentService = commentService;
            this.attachmentService = attachmentService;
            this.userService = userService;
        }
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var user = userService.GetEmployeeIdByUserId(userId);
            var employee = employeeService.GetEmployeeById(user);
            return View(employee);
        }

        [HttpGet]
        public IActionResult DepartmentDetails()
        {
            int? employeeId = HttpContext.Session.GetInt32("UserId");
            if (employeeId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var employee = departmentService.GetAll();
            return View(employee);
        }

        [HttpGet]
        public IActionResult Task()
        {
            int? employeeId = HttpContext.Session.GetInt32("UserId");
            if (employeeId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var employeetask = taskService.GetTaskbyEmployeeId(employeeId);
            if (employeetask == null)
            {
                TempData["Message"] = "You dont have Tasks";
                return RedirectToAction("Index");
            }
            return View(employeetask);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null || id < 0)
            {
                return RedirectToAction("Index", "Employee");
            }
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var Taskemployee = taskService.GetTaskById(id.Value);
            var existingTask = new TaskEditViewModel()
            {
                Title = Taskemployee.Title,
                Description = Taskemployee.Description,
                Status = Taskemployee.Status,
                Id=Taskemployee.Id
            };
            if (Taskemployee.Status == "In Progress") {
                existingTask.Comment = Taskemployee.Comments.Content;
                existingTask.FilePath = Taskemployee.Attachments.FilePath;
                return View(existingTask);
            }
            return View(existingTask);
        }

        [HttpPost]
        public IActionResult Edit(TaskEditViewModel taskEditViewModel)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var existingTask = taskService.GetTaskById(taskEditViewModel.Id);


            if (existingTask == null)
                return Json(new { success=false}); 


            existingTask.Status = taskEditViewModel.Status;
            
            if (existingTask.Status == "In Progress")
            {
                // Save comment
                if (!string.IsNullOrEmpty(taskEditViewModel.Comment))
                {
                    commentService.Add(new Comment
                    {
                        TaskId = existingTask.Id,
                        Content = taskEditViewModel.Comment,
                        UserId = userId.Value

                    });
                }

                // Save attachment
                if (taskEditViewModel.Attachment != null)
                {
                    var filePath = Path.Combine("wwwroot/uploads", taskEditViewModel.Attachment.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        taskEditViewModel.Attachment.CopyTo(stream);
                    }

                    attachmentService.Add(new Attachment
                    {
                        TaskId = existingTask.Id,
                        FilePath = "/uploads/" + taskEditViewModel.Attachment.FileName,
                        UploadedById = userId.Value
                    });


                } 
            }
            int result = taskService.Update(existingTask);
            if (result > 0)
            {
                TempData["Message"] = "status updated successfully.";
                return Json(new { success = true });
            }
            else
            {
                TempData["Message"] = "Failed to update status.";
                return PartialView("_EditTaskPartial", taskEditViewModel);
            }
            
        }
    }
}


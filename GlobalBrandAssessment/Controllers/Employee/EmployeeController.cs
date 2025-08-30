using System;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.AttachmentDTO;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
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
        private readonly IMapper mapper;

        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService, ITaskService taskService, ICommentService commentService, IAttachmentService attachmentService,IUserService userService,IMapper mapper)
        {
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.taskService = taskService;
            this.commentService = commentService;
            this.attachmentService = attachmentService;
            this.userService = userService;
            this.mapper = mapper;
        }
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role=="Manager")
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
            var Role = HttpContext.Session.GetString("Role");
            if (employeeId == null||Role == "Manager")
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
            var Role = HttpContext.Session.GetString("Role");
            if (employeeId == null || Role == "Manager")
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role == "Manager")
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null || id < 0)
            {
                return RedirectToAction("Index", "Employee");
            }
           
            var Taskemployee = taskService.GetTaskById(id.Value);
            var result=mapper.Map<AddandUpdateTaskDTO,TaskEditViewModel>(Taskemployee);
            if (result.Status == "In Progress") {
                return View(result);
            }
            return View(result);
        }

        [HttpPost]
        public IActionResult Edit(TaskEditViewModel taskEditViewModel)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role == "Manager")
            {
                return RedirectToAction("Index", "Login");
            }


            if (taskEditViewModel == null)
                return Json(new { success=false}); 


       
            
            if (taskEditViewModel.Status == "In Progress")
            {
                // Save comment
                if (!string.IsNullOrEmpty(taskEditViewModel.Content))
                {
                    var addCommentDTO = mapper.Map<TaskEditViewModel, AddAndUpdateCommentDTO>(taskEditViewModel);
                    addCommentDTO.UserId = userId.Value;
                    commentService.AddOrUpdate(addCommentDTO);
                }

                // Save attachment
                if (taskEditViewModel.Attachment != null)
                {
                    taskEditViewModel.FilePath = Path.Combine("wwwroot/uploads", taskEditViewModel.Attachment.FileName);
                    using (var stream = new FileStream(taskEditViewModel.FilePath, FileMode.Create))
                    {
                        taskEditViewModel.Attachment.CopyTo(stream);
                    }
                    var attachmentDto=mapper.Map<TaskEditViewModel, AddAndUpdateAttachmentDTO>(taskEditViewModel);
                    attachmentDto.UploadedById = userId.Value;
                    attachmentService.AddOrUpdate(attachmentDto);


                } 
            }
            var AddandUpdateTaskDTO= mapper.Map<TaskEditViewModel, AddandUpdateTaskDTO>(taskEditViewModel);
            AddandUpdateTaskDTO.EmployeeId = userId;
            int result = taskService.Update( AddandUpdateTaskDTO);
            if (result > 0)
            {
                TempData["Message"] = "status updated successfully.";
                return Json(new { success = true ,redirecturl=Url.Action("Task","Employee")});
            }
            else
            {
                TempData["Message"] = "Failed to update status.";
                return PartialView("_EditTaskPartial", taskEditViewModel);
            }
            
        }
    }
}


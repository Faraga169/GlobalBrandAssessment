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
        private readonly ILogger<EmployeeController> logger;

        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService, ITaskService taskService, ICommentService commentService, IAttachmentService attachmentService,IUserService userService,IMapper mapper,ILogger<EmployeeController> logger)
        {
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.taskService = taskService;
            this.commentService = commentService;
            this.attachmentService = attachmentService;
            this.userService = userService;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try {
                int? userId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (userId == null || Role == "Manager")
                {
                    return RedirectToAction("Index", "Login");
                }
                var user = await userService.GetEmployeeIdByUserIdAsync(userId);
                var employee = await employeeService.GetEmployeeByIdAsync(user);
                return View(employee);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Employee Index action.");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> DepartmentDetails()
        {
            try {
                int? employeeId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (employeeId == null || Role == "Manager")
                {
                    return RedirectToAction("Index", "Login");
                }
                var employee = await departmentService.GetAllAsync();
                return View(employee);

            }

            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred in Employee DepartmentDetails action.");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Task()
        {
            try {
                int? employeeId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (employeeId == null || Role == "Manager")
                {
                    return RedirectToAction("Index", "Login");
                }
                var employeetask = await taskService.GetTaskbyEmployeeIdAsync(employeeId);
                if (employeetask == null)
                {
                    TempData["Message"] = "You dont have Tasks";
                    return RedirectToAction("Index");
                }
                return View(employeetask);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Employee Task action.");
                return StatusCode(500, "Internal server error");
            }   

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try {
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

                var Taskemployee = await taskService.GetTaskByIdAsync(id.Value);
                var result = mapper.Map<AddandUpdateTaskDTO, TaskEditViewModel>(Taskemployee);
                if (result.Status == "In Progress")
                {
                    return View(result);
                }
                return View(result);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Employee Edit GET action.");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskEditViewModel taskEditViewModel)
        {
            try {

            int? userId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (userId == null || Role == "Manager")
                {
                    return RedirectToAction("Index", "Login");
                }


                if (taskEditViewModel == null)
                    return Json(new { success = false });




                if (taskEditViewModel.Status == "In Progress")
                {
                    // Save comment
                    if (!string.IsNullOrEmpty(taskEditViewModel.Content))
                    {
                        var existingComment = await commentService.GetByTaskIdAsync(taskEditViewModel.Id);

                        var addCommentDTO = mapper.Map<TaskEditViewModel, AddAndUpdateCommentDTO>(taskEditViewModel);

                        if (existingComment != null)
                        {

                            addCommentDTO.UserId = userId.Value;
                            addCommentDTO.CommentId = existingComment.CommentId;
                            await commentService.UpdateAsync(addCommentDTO);
                        }
                        else
                        {
                            await commentService.AddAsync(addCommentDTO);
                        }

                    }

                    // Save attachment
                    if (taskEditViewModel.Attachment != null)
                    {
                        taskEditViewModel.FilePath = Path.Combine("wwwroot/uploads", taskEditViewModel.Attachment.FileName);
                        using (var stream = new FileStream(taskEditViewModel.FilePath, FileMode.Create))
                        {
                            taskEditViewModel.Attachment.CopyTo(stream);
                        }
                        var existattachment = await attachmentService.GetByTaskIdAsync(taskEditViewModel.Id);
                        var attachmentDto = mapper.Map<TaskEditViewModel, AddAndUpdateAttachmentDTO>(taskEditViewModel);


                        if (existattachment != null)
                        {
                            attachmentDto.UploadedById = userId.Value;
                            attachmentDto.AttachmentId = existattachment.AttachmentId;
                            await attachmentService.UpdateAsync(attachmentDto);
                        }
                        else
                        {
                            await attachmentService.AddAsync(attachmentDto);
                        }



                    }
                }
                var AddandUpdateTaskDTO = mapper.Map<TaskEditViewModel, AddandUpdateTaskDTO>(taskEditViewModel);
                AddandUpdateTaskDTO.EmployeeId = userId;
                int result = await taskService.UpdateAsync(AddandUpdateTaskDTO);
                if (result > 0)
                {
                    TempData["Message"] = "status updated successfully.";
                    return Json(new { success = true, redirecturl = Url.Action("Task", "Employee") });
                }
                else
                {
                    TempData["Message"] = "Failed to update status.";
                    return PartialView("_EditTaskPartial", taskEditViewModel);
                }
            }

            catch (Exception ex) { 
            logger.LogError(ex, "An error occurred in Employee Edit POST action.");
            TempData["Message"] = "Something wrong going";
            return Json(new { success = true, redirecturl = Url.Action("Task", "Employee") });
            }
            
        }
    }
}


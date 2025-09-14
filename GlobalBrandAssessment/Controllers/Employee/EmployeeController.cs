using System;
using System.Security.Claims;
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role == "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            try {
               
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role == "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            try {  
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role == "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            try {
               
                var user = await userService.GetEmployeeIdByUserIdAsync(userId);
                var employeetask = await taskService.GetTaskbyEmployeeIdAsync(user);
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role == "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            try {
                if (!id.HasValue)
                    return BadRequest();
              

                var Taskemployee = await taskService.GetTaskByIdAsync(id.Value);

                if (Taskemployee == null)
                    return NotFound();

                var result = mapper.Map<AddandUpdateTaskDTO, TaskEditViewModel>(Taskemployee);
                
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

            int? userId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (userId == null || Role == "Manager")
            {
                return RedirectToAction("Index", "Login");
            }
            try {

                var user = await userService.GetEmployeeIdByUserIdAsync(userId);
               

                if (taskEditViewModel.Status == "In Progress")
                {
                    // Save comment
                    if (!string.IsNullOrEmpty(taskEditViewModel.Content))
                    {
                        var existingComment = await commentService.GetByTaskIdAsync(taskEditViewModel.Id);

                        var addCommentDTO = mapper.Map<TaskEditViewModel, AddAndUpdateCommentDTO>(taskEditViewModel);

                        if (existingComment != null)
                        {

                            addCommentDTO.EmployeeId = user.Value;
                            addCommentDTO.CommentId = existingComment.CommentId;
                            await commentService.UpdateAsync(addCommentDTO);
                        }
                        else
                        {
                            addCommentDTO.EmployeeId = user.Value;
                            await commentService.AddAsync(addCommentDTO);
                        }

                    }

                    // Save attachment
                    if (taskEditViewModel.Attachment != null)
                    {
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var fileName = Guid.NewGuid() + "_" + Path.GetFileName(taskEditViewModel.Attachment.FileName);
                        var fullPath = Path.Combine(uploadPath, fileName);
                        var existattachment = await attachmentService.GetByTaskIdAsync(taskEditViewModel.Id);
                        if (existattachment != null && !string.IsNullOrEmpty(existattachment.FilePath))
                        {
                            var oldFilePath = Path.Combine(uploadPath, existattachment.FilePath);

                            // احذف القديم لو مختلف عن الجديد
                            if (System.IO.File.Exists(oldFilePath) && existattachment.FilePath != fileName)
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await taskEditViewModel.Attachment.CopyToAsync(stream);
                        }

                        taskEditViewModel.FilePath = fileName;
                    
                        var attachmentDto = mapper.Map<TaskEditViewModel, AddAndUpdateAttachmentDTO>(taskEditViewModel);

                    if (existattachment != null)
                        {
                            attachmentDto.EmployeeId= user.Value;
                            attachmentDto.AttachmentId = existattachment.AttachmentId;
                         
                            await attachmentService.UpdateAsync(attachmentDto);
                        }
                        else
                        {
                            attachmentDto.EmployeeId = user.Value;
                            await attachmentService.AddAsync(attachmentDto);
                        }



                    }
                }
                var AddandUpdateTaskDTO = mapper.Map<TaskEditViewModel, AddandUpdateTaskDTO>(taskEditViewModel);
                AddandUpdateTaskDTO.EmployeeId = user;
                int result = await taskService.UpdateAsync(AddandUpdateTaskDTO);
                if (result > 0)
                {
                    TempData["Message"] = "status updated successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Task", "Employee") });
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
            return Json(new { success = true, redirectUrl = Url.Action("Task", "Employee") });
            }
            
        }
        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","uploads", fileName.TrimStart('/'));

            // force download for any file type
            var contentType = "application/octet-stream";

            return PhysicalFile(fullPath, contentType, fileName);
        }
    }
}


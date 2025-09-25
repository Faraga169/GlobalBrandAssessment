using System;
using System.Net.Mail;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlobalBrandAssessment.PL.Controllers.Employee
{

    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly UserManager<User> userManager;
        private readonly IDepartmentService departmentService;
        private readonly ITaskService taskService;
        private readonly ICommentService commentService;
        private readonly IAttachmentService attachmentService;
      
        private readonly IMapper mapper;
        private readonly ILogger<EmployeeController> logger;

        public EmployeeController(IEmployeeService employeeService,UserManager<User> userManager, IDepartmentService departmentService, ITaskService taskService, ICommentService commentService, IAttachmentService attachmentService,IMapper mapper,ILogger<EmployeeController> logger)
        {
            this.employeeService = employeeService;
            this.userManager = userManager;
            this.departmentService = departmentService;
            this.taskService = taskService;
            this.commentService = commentService;
            this.attachmentService = attachmentService;
            
            this.mapper = mapper;
            this.logger = logger;
        }
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
           

            try {
                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                int? employeeId = currentUser.EmployeeId;
                var employee = await employeeService.GetEmployeeByIdAsync(employeeId);
                return View(employee);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Employee Index action.");
                return StatusCode(500, "Internal server error");
            }

        }


        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> Task()
        {
            

            try {

                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                int? employeeId = currentUser.EmployeeId;
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

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
           

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
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(TaskEditViewModel taskEditViewModel)
        {

            try {
             
                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                int? employeeId = currentUser.EmployeeId;


                if (taskEditViewModel.Status == "In Progress")
                {
                    // Save comment
                    if (!string.IsNullOrEmpty(taskEditViewModel.Content))
                    {
                        var existingComment = await commentService.GetByTaskIdAsync(taskEditViewModel.Id);

                        var addCommentDTO = mapper.Map<TaskEditViewModel, AddAndUpdateCommentDTO>(taskEditViewModel);

                        if (existingComment != null)
                        {

                            addCommentDTO.EmployeeId = employeeId;
                            addCommentDTO.CommentId = existingComment.CommentId;
                            await commentService.UpdateAsync(addCommentDTO);
                        }
                        else
                        {
                            addCommentDTO.EmployeeId = employeeId;
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

                         
                            if (System.IO.File.Exists(oldFilePath))
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
                        attachmentDto.EmployeeId = employeeId;

                        if (existattachment != null)
                        {
                         
                            attachmentDto.AttachmentId = existattachment.AttachmentId;
                            attachmentDto.FilePath = fileName;
                            await attachmentService.UpdateAsync(attachmentDto);
                        }
                        else
                        {
                            attachmentDto.FilePath = fileName;
                            await attachmentService.AddAsync(attachmentDto);
                        }
                    }

                }
                var AddandUpdateTaskDTO = mapper.Map<TaskEditViewModel, AddandUpdateTaskDTO>(taskEditViewModel);
                AddandUpdateTaskDTO.EmployeeId = employeeId;
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
        [Authorize(Roles = "Employee,Manager,Admin")]
        public IActionResult DownloadFile(string fileName)
        {
           
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","uploads", fileName);

            // force download for any file type
            var contentType = "application/octet-stream";
            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found on server.");

            var originalFileName = fileName.Contains('_')
                ? fileName.Substring(fileName.IndexOf('_') + 1)
                : fileName;
            return PhysicalFile(fullPath, contentType, originalFileName);
        }
    }
}


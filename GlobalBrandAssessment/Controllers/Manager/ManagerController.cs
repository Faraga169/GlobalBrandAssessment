using GlobalBrandAssessment.BL.Services.Manager;

using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.BL.Services;
using System.Threading.Tasks;
using GlobalBrandAssessment.GlobalBrandDbContext;
using System.Data;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GlobalBrandAssessment.PL.Controllers.Employee
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly IManagerService managerService;
        private readonly UserManager<User> userManager;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IMapper mapper;
        private readonly ILogger<ManagerController> logger;
        private readonly IWebHostEnvironment environment;

        public ManagerController(IManagerService managerService, UserManager<User> userManager, IEmployeeService employeeService, IDepartmentService departmentService, IMapper mapper, ILogger<ManagerController> logger,IWebHostEnvironment environment)
        {
            this.managerService = managerService;
            this.userManager = userManager;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.mapper = mapper;
            this.logger = logger;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageno=1,int pagesize=5,string sortcolumn="FirstName")

        {

            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;  
            try
            {
                var manager = await employeeService.GetEmployeesByManagerPagedAsync(managerId,pageno,pagesize,sortcolumn);
                return View(manager);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "An error occurred in Manager Index action.");
                return PartialView("Errorpartial", ex);
            }

        }



        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var managerId = currentUser?.EmployeeId;

            try
            {

                var manager = await managerService.SearchAsync(searchname, managerId);
                if (manager == null || !manager.Any())
                {
                    return PartialView("_IndexManagerPartial", new List<GetAllAndSearchManagerDTO>());
                }

                return PartialView("_IndexManagerPartial", manager);


            }
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    TempData["Message"] = ex.Message;
                    return PartialView("_IndexManagerPartial", new List<GetAllAndSearchManagerDTO>());
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

           
            try
            {
                var departments = await departmentService.GetAllAsync();
                ViewBag._Department = new SelectList(departments, "Id", "Name");
                return View();
            }
           
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    TempData["Message"] = ex.Message;
                    return View();
                }
                else
                {
                    //2. Deployment => Log Error In File | Table in Database And Return Error View
                    logger.LogError(ex.Message);
                    return PartialView("Errorpartial",ex);
                }


            }

        }



        [HttpPost]
        public async Task<IActionResult> Create(AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
          


            var manager = await managerService.GetManagerByDepartmentIdAsync(addORUpdateManagerDTO.DeptId);
            if (manager == null)
            {
                ModelState.AddModelError("", "No manager found for the selected department.");
            }
            else
            {
                addORUpdateManagerDTO.ManagerId = manager.Id;
            }


            if (ModelState.IsValid)
            {
                try
                {

                    if (addORUpdateManagerDTO.Image != null)
                    {
                        string rootPath = Directory.GetCurrentDirectory();
                        string wwwRootPath = Path.Combine(rootPath, "wwwroot");
                        string fileName = Path.GetFileNameWithoutExtension(addORUpdateManagerDTO.Image.FileName);
                        string extension = Path.GetExtension(addORUpdateManagerDTO.Image.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await addORUpdateManagerDTO.Image.CopyToAsync(fileStream);
                        }
                        addORUpdateManagerDTO.ImageURL = "/images/" + fileName;
                    }

                    int newEmployeeId = await managerService.Add(addORUpdateManagerDTO);
                    
                    if (newEmployeeId > 0)
                    {
                        var user = mapper.Map<AddAndUpdateManagerDTO, User>(addORUpdateManagerDTO);
                       
                        user.EmployeeId = newEmployeeId;
                        user.Email = $"{user.UserName}@test.com";  
                        user.EmailConfirmed = true;
                        user.Id = Guid.NewGuid().ToString();

                        var result = await userManager.CreateAsync(user, addORUpdateManagerDTO.Password);

                        if (result.Succeeded)
                        {
                          
                            await userManager.AddToRoleAsync(user, "Employee");
                        }
                        TempData["Message"] = "Employee created successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to create employee.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
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
                var departments = await departmentService.GetAllAsync();
                ViewBag._Department = new SelectList(departments, "Id", "Name");
                return PartialView("_CreateManagerPartial", addORUpdateManagerDTO);

            }

        
        

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            
            try
            {
                
                if(!id.HasValue)
                return BadRequest();

                var employee = await employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound();
                var result = mapper.Map<DAL.Data.Models.Employee, AddAndUpdateManagerDTO>(employee);
                if (result == null)
                {
                    TempData["Message"] = "Employee not found.";
                    return RedirectToAction("Index");
                }
                var departments = await departmentService.GetAllAsync();
                ViewBag._Department = new SelectList(departments, "Id", "Name", employee.DeptId);
                return View(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Edit Get action.");
                return PartialView("Errorpartial",ex);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute]int id,AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
            
               
                ModelState.Remove("Password");
                var manager = await managerService.GetManagerByDepartmentIdAsync(addORUpdateManagerDTO.DeptId);
                if (manager == null)
                {
                    ModelState.AddModelError("", "No manager found for the selected department.");
                }
                else
                {
                    addORUpdateManagerDTO.ManagerId = manager.Id;
                }
                var oldImageUrl = await employeeService.GetEmployeeImageUrlAsync(addORUpdateManagerDTO.Id);
            addORUpdateManagerDTO.Id = id;

            if (ModelState.IsValid)
            {
                try
                {
                  

                    if (addORUpdateManagerDTO.Image != null)
                    {

                        // Save new image
                        string rootPath = Directory.GetCurrentDirectory();
                        string wwwRootPath = Path.Combine(rootPath, "wwwroot");
                        string fileName = Path.GetFileNameWithoutExtension(addORUpdateManagerDTO.Image.FileName);
                        string extension = Path.GetExtension(addORUpdateManagerDTO.Image.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            addORUpdateManagerDTO.Image.CopyTo(fileStream);
                        }
                        addORUpdateManagerDTO.ImageURL = "/images/" + fileName;

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(oldImageUrl))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, oldImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                    }
                    else
                    {


                        addORUpdateManagerDTO.ImageURL = oldImageUrl;

                    }

                    int result = await managerService.UpdateAsync(addORUpdateManagerDTO);

                    if (result > 0)
                    {
                     
                        var user = await userManager.Users
                            .FirstOrDefaultAsync(u => u.EmployeeId == addORUpdateManagerDTO.Id);

                        if (user != null)
                        {

                            user.UserName = addORUpdateManagerDTO.FirstName;
                            user.Email = $"{addORUpdateManagerDTO.FirstName}@test.com";
                            user.EmailConfirmed = true;
                            var updateResult = await userManager.UpdateAsync(user);
                        }

                         TempData["Message"] = "Employee updated successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to update employee.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });

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
                        logger.LogError(ex, "Error happened while updating Employee {EmployeeId}",addORUpdateManagerDTO.Id);
                        return PartialView("Errorpartial", ex);
                    }

                }
            }

                var departments = await departmentService.GetAllAsync();
                ViewBag._Department = new SelectList(departments, "Id", "Name", addORUpdateManagerDTO.DeptId);
                return PartialView("_EditManagerPartial", addORUpdateManagerDTO);

            }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
               if(!id.HasValue)
                    return BadRequest();

                var result = await managerService.DeleteAsync(id);
                if (result > 0)
                {

                    var user = await userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId ==id);

                    if (user != null) {
                        await userManager.DeleteAsync(user);
                      
                    }

                        TempData["Message"] = "Employee deleted successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
                }
                else
                {
                    TempData["Message"] = "Employee deleted failed.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Delete action.");
                TempData["Message"] = "Failed to delete employee.something wrong";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
            }

        }
    }
}
         
        
    


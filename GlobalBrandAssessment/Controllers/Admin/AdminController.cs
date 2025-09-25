using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Data.Models.Common;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.PL.Controllers.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GlobalBrandAssessment.PL.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IManagerService managerService;
        private readonly UserManager<User> userManager;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IMapper mapper;
        private readonly ILogger<ManagerController> logger;
        private readonly IWebHostEnvironment environment;
        private readonly ITaskService taskService;

        public AdminController(IManagerService managerService, UserManager<User> userManager, IEmployeeService employeeService, IDepartmentService departmentService, IMapper mapper, ILogger<ManagerController> logger, IWebHostEnvironment environment,ITaskService taskService)
        {
            this.managerService = managerService;
            this.userManager = userManager;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.mapper = mapper;
            this.logger = logger;
            this.environment = environment;
            this.taskService = taskService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName")
        {
            try
            {
                var manager = await employeeService.GetAllPagedAsync(pageno, pagesize, sortcolumn);
                return View(manager);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "An error occurred in Admin Index action.");
                return PartialView("Errorpartial", ex);
            }


        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
           

            try
            {

                var manager = await managerService.SearchAsync(searchname,null);
                if (manager == null || !manager.Any())
                {
                    return PartialView("_IndexAdminPartial", new List<GetAllAndSearchManagerDTO>());
                }

                return PartialView("_IndexAdminPartial", manager);


            }
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    // 1.Development => Log Error In Console and Return Same view with Error Message
                    TempData["Message"] = ex.Message;
                    return PartialView("_IndexAdminPartial", new List<GetAllAndSearchManagerDTO>());
                }
                else
                {
                    //2. Deployment => Log Error In File | Table in Database And Return Error View
                    logger.LogError(ex.Message);
                    return PartialView("Errorpartial", ex);
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
                var Roles = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(r => new SelectListItem
                {
                    Value = r.ToString(),
                    Text = r.ToString()
                }).ToList();
                ViewBag.Roles = new SelectList(Roles, "Value", "Text");
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
                    return PartialView("Errorpartial", ex);
                }


            }

        }



        [HttpPost]
        public async Task<IActionResult> Create(AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {


            var department = await departmentService.GetDepartmentByIdAsync(addORUpdateManagerDTO.DeptId);
            if (addORUpdateManagerDTO.Role == "Employee")
            {
                var manager = await managerService.GetManagerByDepartmentIdAsync(addORUpdateManagerDTO.DeptId);

                if (manager == null)
                {
                    addORUpdateManagerDTO.ManagerId = null;
                }
                else {
                    addORUpdateManagerDTO.ManagerId = manager?.Id;
                }
          

            }


            if (addORUpdateManagerDTO.Role == "Manager")
            {
                if (department != null && department.ManagerId != null)
                {
                    ModelState.AddModelError("", "This Department Already Has a Manager assigned");
                }

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

                            await userManager.AddToRoleAsync(user, addORUpdateManagerDTO.Role);
                            if (addORUpdateManagerDTO.Role == "Manager" && addORUpdateManagerDTO.DeptId.HasValue)
                            {
                                if (department != null && department.ManagerId == null)
                                {
                                    department.ManagerId = newEmployeeId;
                                    await departmentService.UpdateAsync(department);
                                   
                                    await managerService.DemoteManagerToEmployeeAsync(department.ManagerId);
                                   
                                }
                            }
                        }
                        TempData["Message"] = "Employee created successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to create employee.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
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
            var Roles = Enum.GetValues(typeof(Role))
           .Cast<Role>()
           .Select(r => new SelectListItem
           {
               Value = r.ToString(),
               Text = r.ToString()
           }).ToList();
            ViewBag.Roles = new SelectList(Roles, "Value", "Text");
            return PartialView("_CreateAdminPartial", addORUpdateManagerDTO);

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            try
            {

                if (!id.HasValue)
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
                var Roles = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(r => new SelectListItem
                {
                    Value = r.ToString(),
                    Text = r.ToString()
                }).ToList();
                ViewBag.Roles = new SelectList(Roles, "Value", "Text");
                return View(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Edit Get action.");
                return PartialView("Errorpartial", ex);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {


            ModelState.Remove("Password");
            var department = await departmentService.GetDepartmentByIdAsync(addORUpdateManagerDTO.DeptId);
            var manager = await managerService.GetManagerByDepartmentIdAsync(addORUpdateManagerDTO.DeptId);


            if (addORUpdateManagerDTO.Role == "Manager")
            {
                if (department != null && department.ManagerId != null)
                {
                    ModelState.AddModelError("", "This Department already has another Manager.");
                }
                else
                {
                    department.ManagerId = addORUpdateManagerDTO.Id;
                    await departmentService.UpdateAsync(department);
                    
                    await managerService.DemoteManagerToEmployeeAsync(department.ManagerId);
                    addORUpdateManagerDTO.ManagerId = null;
                   
                }
            }
            else if (addORUpdateManagerDTO.Role == "Employee")
            {
                if (department != null && department.ManagerId == addORUpdateManagerDTO.Id)
                {
                    await managerService.DemoteManagerToEmployeeAsync(department.ManagerId);
                    department.ManagerId = null;
                    await departmentService.UpdateAsync(department);
                  
                 
                }
                else
                {
                    addORUpdateManagerDTO.ManagerId = manager?.Id; 
                }
            }
            var oldImageUrl = await employeeService.GetEmployeeImageUrlAsync(addORUpdateManagerDTO.Id);
            

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

                            if (updateResult.Succeeded)
                            {

                                var currentRoles = await userManager.GetRolesAsync(user);


                                if (currentRoles.Any())
                                {
                                    await userManager.RemoveFromRolesAsync(user, currentRoles);
                                }


                                if (!string.IsNullOrEmpty(addORUpdateManagerDTO.Role))
                                {
                                    await userManager.AddToRoleAsync(user, addORUpdateManagerDTO.Role);
                                }
                            }
                        }

                        TempData["Message"] = "Employee updated successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                    }
                    else
                    {
                        TempData["Message"] = "Failed to update employee.";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });

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
                        logger.LogError(ex, "Error happened while updating Employee {EmployeeId}", addORUpdateManagerDTO.Id);
                        return PartialView("Errorpartial", ex);
                    }

                }
            }

            var departments = await departmentService.GetAllAsync();
            ViewBag._Department = new SelectList(departments, "Id", "Name", addORUpdateManagerDTO.DeptId);
            var Roles = Enum.GetValues(typeof(Role))
                 .Cast<Role>()
                 .Select(r => new SelectListItem
                 {
                     Value = r.ToString(),
                     Text = r.ToString()
                 }).ToList();
            ViewBag.Roles = new SelectList(Roles, "Value", "Text", addORUpdateManagerDTO.Role);
            return PartialView("_EditAdminPartial", addORUpdateManagerDTO);

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (!id.HasValue)
                    return BadRequest();

             
                var result = await managerService.DeleteAsync(id);

                if (result > 0)
                {
                   
                    var user = await userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == id);

                    if (user != null)
                    {
                        var userResult = await userManager.DeleteAsync(user);
                        if (!userResult.Succeeded)
                        {
                            TempData["Message"] = "Employee deleted but failed to delete related user.";
                            return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                        }
                    }

                    TempData["Message"] = "Employee deleted successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                }
                else
                {
                    if (result == -1)
                        TempData["Message"] = "Can't delete this employee because he has employees under him.";
                    else if (result == -2)
                        TempData["Message"] = "Can't delete this employee because he is assigned as a manager of a department.";
                    else
                        TempData["Message"] = "Employee delete failed.";

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Delete action.");
                TempData["Message"] = "Failed to delete employee. Something went wrong.";
                return Json(new { success = false, redirectUrl = Url.Action("Index", "Admin") });
            }
        }



    }
}

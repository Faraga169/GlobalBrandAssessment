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

namespace GlobalBrandAssessment.PL.Controllers.Employee
{
    public class ManagerController : Controller
    {
        private readonly IManagerService managerService;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly ILogger<ManagerController> logger;
        private readonly IWebHostEnvironment environment;

        public ManagerController(IManagerService managerService, IEmployeeService employeeService, IDepartmentService departmentService, IUserService userService, IMapper mapper, ILogger<ManagerController> logger,IWebHostEnvironment environment)
        {
            this.managerService = managerService;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.userService = userService;
            this.mapper = mapper;
            this.logger = logger;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()

        {

            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var manager = await employeeService.GetEmployeesByManagerAsync(mangerId);
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

            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {

                var manager = await managerService.SearchAsync(searchname, mangerId);
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

            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
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

            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }


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
                        user.Role = "Employee";
                        user.EmployeeId = newEmployeeId;

                        await userService.AddAsync(user);
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
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
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
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
           
               
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
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
               if(!id.HasValue)
                    return BadRequest();

                var result = await managerService.DeleteAsync(id);
                if (result > 0)
                {

                   await userService.RemoveAsync(id);
                    
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
         
        
    


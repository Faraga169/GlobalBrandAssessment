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

        public ManagerController(IManagerService managerService, IEmployeeService employeeService, IDepartmentService departmentService, IUserService userService, IMapper mapper, ILogger<ManagerController> logger)
        {
            this.managerService = managerService;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.userService = userService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()

        {
            try
            {

                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }

                var manager = await employeeService.GetEmployeesByManagerAsync(mangerId);
                return View(manager);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Index action.");
                return StatusCode(500, "Internal server error");
            }

        }



        [HttpPost]
        public async Task<IActionResult> Search(string searchname)
        {
            try
            {

                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
                var manager = await managerService.SearchAsync(searchname, mangerId);
                if (manager == null || !manager.Any())
                {
                    return PartialView("_IndexManagerPartial", new List<GetAllAndSearchManagerDTO>());
                }


                return PartialView("_IndexManagerPartial", manager);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Search action.");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
                var departments = await departmentService.GetAllAsync();
                ViewBag._Department = new SelectList(departments, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Create Get action.");
                return StatusCode(500, "Internal server error");
            }

        }



        [HttpPost]
        public async Task<IActionResult> Create(AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
            try
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

                    int newEmployeeId = await managerService.AddAsync(addORUpdateManagerDTO);
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
                var departments = await departmentService.GetAllAsync();
                ViewBag._Department = new SelectList(departments, "Id", "Name");
                return PartialView("_CreateManagerPartial", addORUpdateManagerDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Create Post action.");
                TempData["Message"] = "Failed to create employee something going wrong";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
            }

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
                if (id is null || id < 0)
                {
                    return RedirectToAction("Index", "Manager");
                }

                var employee = await employeeService.GetEmployeeByIdAsync(id);
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
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
            try
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


                if (ModelState.IsValid)
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
                        addORUpdateManagerDTO.ImageURL = addORUpdateManagerDTO.ImageURL = "/images/" + fileName;
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

                var departments = await departmentService.GetAllAsync();
                ViewBag._Department = new SelectList(departments, "Id", "Name", addORUpdateManagerDTO.DeptId);
                return PartialView("_EditManagerPartial", addORUpdateManagerDTO);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Manager Edit Post action.");

                TempData["Message"] = "Failed to update employee.something wrong";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
            }

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {

                int? mangerId = HttpContext.Session.GetInt32("UserId");
                var Role = HttpContext.Session.GetString("Role");
                if (mangerId == null || Role == "Employee")
                {
                    return RedirectToAction("Index", "Login");
                }
                if (id is null)
                {
                    TempData["Message"] = "Invalid employee id.";
                    return Json(new { success = false });
                }

                var result = await managerService.DeleteAsync(id);
                if (result > 0)
                {
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
         
        
    


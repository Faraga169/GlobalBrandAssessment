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

        public ManagerController(IManagerService managerService,IEmployeeService employeeService,IDepartmentService departmentService,IUserService userService,IMapper mapper)
        {
            this.managerService = managerService;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
            this.userService = userService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }

            var manager = employeeService.GetEmployeesByManager(mangerId).ToList();
            return View(manager);
        }



        [HttpPost]
        public IActionResult Search(string searchname)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            var manager = managerService.Search(searchname, mangerId).ToList();
            if (manager == null || !manager.Any())
            {
                return PartialView("_IndexManagerPartial", new List<GetAllAndSearchManagerDTO>());
            }
            
            
            return PartialView("_IndexManagerPartial",manager);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name");
            return View();
        }



        [HttpPost]
        public IActionResult Create(AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
            
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
         
            addORUpdateManagerDTO.ManagerId = managerService.GetManagerByDepartmentId(addORUpdateManagerDTO.DeptId).Id;

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
                        addORUpdateManagerDTO.Image.CopyTo(fileStream);
                    }
                    addORUpdateManagerDTO.ImageURL = "/images/" + fileName;
                }

                int newEmployeeId = managerService.Add(addORUpdateManagerDTO);
                if (newEmployeeId > 0)
                {
                    var user=mapper.Map<AddAndUpdateManagerDTO, User>(addORUpdateManagerDTO);
                    user.Role = "Employee";
                    user.EmployeeId = newEmployeeId;

                    userService.Add(user);
                    TempData["Message"] = "Employee created successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
                }
                else
                {
                    TempData["Message"] = "Failed to create employee.";
                    return Json(new { success = false, redirectUrl = Url.Action("Index", "Manager") });
                }
            }
            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name");
            return PartialView("_CreateManagerPartial", addORUpdateManagerDTO);
        }


        [HttpGet]
        public IActionResult Edit(int? id) {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null || id<0) { 
            return RedirectToAction("Index", "Manager");
            }
           
            var employee = employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                TempData["Message"] = "Employee not found.";
                return RedirectToAction("Index");
            }
            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name",employee.DeptId);
            return View(employee);
        }

        [HttpPost]
        public IActionResult Edit(AddAndUpdateManagerDTO addORUpdateManagerDTO)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            addORUpdateManagerDTO.ManagerId = managerService.GetManagerByDepartmentId(addORUpdateManagerDTO.DeptId).Id;
            ModelState.Remove("Password");
           
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
                    addORUpdateManagerDTO.ImageURL= addORUpdateManagerDTO.ImageURL = "/images/" + fileName;
                }

                int result = managerService.Update(addORUpdateManagerDTO);

                if (result > 0)
                {
                    TempData["Message"] = "Employee updated successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
                }
                else
                {
                    TempData["Message"] = "Failed to update employee.";
                    return Json(new { success = false, redirectUrl = Url.Action("Index", "Manager") });
                    
                }
              
              
            }

            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name", addORUpdateManagerDTO.DeptId);
            return PartialView("_EditManagerPartial", addORUpdateManagerDTO);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            var Role = HttpContext.Session.GetString("Role");
            if (mangerId == null || Role == "Employee")
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null)
            {
                TempData["Message"] =  "Invalid employee id.";
                return Json(new { success = false});
            }

            var result = managerService.Delete(id);
            if (result > 0)
            {
                TempData["Message"] = "Employee deleted successfully.";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Manager") });
            }
            else
            {
                TempData["Message"] = "Employee deleted failed.";
                return Json(new { success = false, redirectUrl = Url.Action("Index", "Manager") });
            }
        }
    }
}
         
        
    


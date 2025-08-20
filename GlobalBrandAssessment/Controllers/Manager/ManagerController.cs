using GlobalBrandAssessment.BL.Services.Manager;

using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.BL.Services;
using System.Threading.Tasks;

namespace GlobalBrandAssessment.PL.Controllers.Employee
{
    public class ManagerController : Controller
    {
        private readonly IManagerService managerService;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;

        public ManagerController(IManagerService managerService,IEmployeeService employeeService,IDepartmentService departmentService)
        {
            this.managerService = managerService;
            this.employeeService = employeeService;
            this.departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var manager = employeeService.GetEmployeesByManager(mangerId.Value).ToList();
            return View(manager);
        }



      
        public IActionResult Search(string searchname)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var manager = managerService.Search(searchname, mangerId).ToList();
            if (manager == null || !manager.Any())
            {
                return PartialView("_IndexManagerPartial", new List<DAL.Data.Models.Employee>());
            }
            
            
            return PartialView("_IndexManagerPartial",manager);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name");
            return View();
        }



        [HttpPost]
        public IActionResult Create(DAL.Data.Models.Employee employee)
        {
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return Json(new { success = false, message = "Session expired, please login again." });
            }

            employee.ManagerId = mangerId;

            if (ModelState.IsValid)
            {
                if (employee.Image != null)
                {
                    string rootPath = Directory.GetCurrentDirectory();
                    string wwwRootPath = Path.Combine(rootPath, "wwwroot");
                    string fileName = Path.GetFileNameWithoutExtension(employee.Image.FileName);
                    string extension = Path.GetExtension(employee.Image.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        employee.Image.CopyTo(fileStream);
                    }
                    employee.ImageURL = "/images/" + fileName;
                }

                int result = managerService.Add(employee);

                if (result > 0)
                {
                    TempData["Message"] = "Employee created successfully.";
                    return Json(new { success = true });
                }
                else
                {
                    TempData["Message"] = "Failed to create employee.";
                    return Json(new { success = false});
                }
            }
            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name");
            return PartialView("_CreateManagerPartial", employee);
        }


        [HttpGet]
        public IActionResult Edit(int? id) {
            if (id is null || id<0) { 
            return RedirectToAction("Index", "Manager");
            }
            int? mangerId = HttpContext.Session.GetInt32("UserId");
            if (mangerId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var employee = employeeService.GetEmployeeById(id.Value);
            if (employee == null)
            {
                TempData["Message"] = "Employee not found.";
                return RedirectToAction("Index");
            }
            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name",employee.DeptId);
            return View(employee);
        }

        [HttpPost]
        public IActionResult Edit(DAL.Data.Models.Employee employee)
        {

            var existingEmployee = employeeService.GetEmployeeById(employee.Id);
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.DeptId = employee.DeptId;
            existingEmployee.ManagerId = managerService.GetManagerByDepartmentId(employee.DeptId).Id;

            if (existingEmployee == null)
                return PartialView("_EditManagerPartial", existingEmployee);




            if (ModelState.IsValid)
            {
                
                
                if (employee.Image != null)
                {
                    // Save new image
                    string rootPath = Directory.GetCurrentDirectory();
                    string wwwRootPath = Path.Combine(rootPath, "wwwroot");
                    string fileName = Path.GetFileNameWithoutExtension(employee.Image.FileName);
                    string extension = Path.GetExtension(employee.Image.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        employee.Image.CopyTo(fileStream);
                    }
                    existingEmployee.ImageURL= employee.ImageURL = "/images/" + fileName;
                }

               
                int result = managerService.Update(existingEmployee);
                if (result > 0)
                {
                    TempData["Message"] = "Employee updated successfully.";
                    return Json(new { success = true });
                }
                else
                {
                    TempData["Message"] = "Failed to update employee.";
                    return Json(new { success = false });
                    
                }
              
              
            }

            ViewBag._Department = new SelectList(departmentService.GetAll(), "Id", "Name", employee.DeptId);
            return PartialView("_EditManagerPartial", existingEmployee);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                TempData["Message"] =  "Invalid employee id.";
                return Json(new { success = false});
            }

            var manager = HttpContext.Session.GetInt32("UserId");
            if (manager is null)
            {
                return RedirectToAction("Index", "Login");
            }

            var result = managerService.Delete(id);
            if (result > 0)
            {
                TempData["Message"] = "Employee deleted successfully.";
                return Json(new { success = true});
            }
            else
            {
                TempData["Message"] = "Employee deleted failed.";
                return Json(new { success = false});
            }
        }
    }
}
         
        
    


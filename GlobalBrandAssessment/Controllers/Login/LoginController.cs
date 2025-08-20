using GlobalBrandAssessment.GlobalBrandDbContext;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GlobalBrandAssessment.PL.Controllers.Login
{
    public class LoginController : Controller
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public LoginController(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = globalbrandDbContext.Users
                    .FirstOrDefault(u => u.UserName == loginViewModel.UserName
                                      && u.Password == loginViewModel.Password);

                if (user == null)
                {
                    ViewBag.error = "UserName not exist in System";
                    return View(loginViewModel);
                }

                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Role", user.Role);

                if (user.Role == "Manager")
                {
                    return RedirectToAction("Index", "Manager");
                }
                else if (user.Role == "Employee")
                {
                    return RedirectToAction("Index", "Employee");
                }
            }

            return View(loginViewModel);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

    }
}

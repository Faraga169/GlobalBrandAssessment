using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GlobalBrandAssessment.PL.Controllers.Login
{
    public class LoginController : Controller
    {
       
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public LoginController(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(); 
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
                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Login")
               .Information("Login attempt started for user: {UserName}", loginViewModel.UserName);

                var username = userManager.FindByNameAsync(loginViewModel.UserName).Result;
                if (username == null)
                {
                    Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Login")
              .Warning("Login failed: Username {UserName} does not exist.", loginViewModel.UserName);
                    ModelState.AddModelError("", "Username does not exist");
                    return View(loginViewModel);
                }

                var passwordValid = userManager.CheckPasswordAsync(username, loginViewModel.Password).Result;
                if (!passwordValid)
                {
                    Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Login")
              .Warning("Login failed: Invalid password for user {UserName}.", loginViewModel.UserName);
                    ModelState.AddModelError("", "Invalid password");
                    return View(loginViewModel);
                }

                var result = signInManager.PasswordSignInAsync(username, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false).Result;

                if (result.IsNotAllowed)
                {
                    Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Login")
              .Warning("Login blocked: User {UserName} is not allowed to log in.", loginViewModel.UserName);
                    ModelState.AddModelError("", "You are not allowed to login");
                    return View(loginViewModel);
                }

                if (result.IsLockedOut)
                {
                    Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Login")
              .Warning("Login blocked: User {UserName} account is locked out.", loginViewModel.UserName);
                    ModelState.AddModelError("", "You are locked out");
                    return View(loginViewModel);
                }

                if (result.Succeeded)
                {
                    var userRole = userManager.GetRolesAsync(username).Result.FirstOrDefault();
                    Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Login")
               .Information("User {UserName} logged in successfully with role '{Role}'.", loginViewModel.UserName, userRole);

                    if (userRole == "Manager")
                        return RedirectToAction("Index", "Manager");
                    else if (userRole == "Employee")
                        return RedirectToAction("Index", "Employee");
                    else if (userRole == "Admin")
                        return RedirectToAction("Index", "Admin");
                }

                Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "Index")
              .ForContext("Controller", "Login")
               .Warning("Login failed for user '{UserName}' due to unknown reason.", loginViewModel.UserName);
            }

            return View(loginViewModel);
            

          

        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name ?? "Unknown";
            await signInManager.SignOutAsync();
            Log.ForContext("UserName", User?.Identity?.Name)
              .ForContext("ActionType", "LogOut")
              .ForContext("Controller", "Login")
            .Information("{UserName} logged out successfully.",userName);
            return RedirectToAction("Index", "Login");
        }

    }
}

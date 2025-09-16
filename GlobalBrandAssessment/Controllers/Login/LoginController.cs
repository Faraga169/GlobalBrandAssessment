using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlobalBrandAssessment.PL.Controllers.Login
{
    public class LoginController : Controller
    {
       
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger<LoginController> logger;

        public LoginController(UserManager<User> userManager,SignInManager<User> signInManager, ILogger<LoginController> logger)
        {
            
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
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
            try
            {

                if (ModelState.IsValid)
                {
                    var username = userManager.FindByNameAsync(loginViewModel.UserName).Result;
                    if (username == null) {
                        ModelState.AddModelError("","Username is not exist");
                        return View(loginViewModel);
                    }

                    var passwordValid = userManager.CheckPasswordAsync(username, loginViewModel.Password).Result;
                    if (!passwordValid) {
                        ModelState.AddModelError("", "Password is not registered in system");
                        return View(loginViewModel);
                    }
                     
                    var result=signInManager.PasswordSignInAsync(username, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false).Result;
                    if(!result.IsNotAllowed)
                        ModelState.AddModelError("", "You are not allowed to login");
                    if(result.IsLockedOut)
                        ModelState.AddModelError("", "You are locked out");
                    if (result.Succeeded) { 
                        var UserRole = userManager.GetRolesAsync(username).Result.FirstOrDefault();
                        if(UserRole=="Manager")
                            return RedirectToAction("Index", "Manager");
                        else if (UserRole == "Employee")
                            return RedirectToAction("Index", "Employee");

                    }

                }

                return View(loginViewModel);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Login Index action.");
                return StatusCode(500, "Server is not work");
            }

        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
           
            await signInManager.SignOutAsync();


            return RedirectToAction("Index", "Login");
        }

    }
}

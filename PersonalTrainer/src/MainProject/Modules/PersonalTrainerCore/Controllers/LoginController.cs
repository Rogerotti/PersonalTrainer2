using Framework.Models.Dto;
using Framework.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace PersonalTrainerCore.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserManagement userManagement;
        private readonly ILogger<LoginController> logger;

        public LoginController(IUserManagement userManagement,
             ILogger<LoginController> logger)
        {
            this.userManagement = userManagement;
            this.logger = logger;
            
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new UserDto());
        }

        [HttpPost]
        public IActionResult Index(UserDto user)
        {
            try
            {
                userManagement.Login(user.Login, user.Password);
            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
                logger.LogDebug("Logowanie przez użytkownika", new[] { exc.Message });
                return View(user);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            try
            {
                userManagement.Login("Rogerottii", "Roger!994");
            }
            catch (Exception)
            {
                userManagement.RegisterUser("Rogerottii", "bb@gmail.com", "Roger!994",0, 122, 122, 22);
                userManagement.Login("Rogerottii", "Roger!994");
               // ModelState.AddModelError("AdditionalValidation", exc.Message);
              //  logger.LogDebug("Logowanie przez administratora", new[] { exc.Message });
            }
         
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            try
            {
                userManagement.Logout();
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("AdditionalValidation", exc.Message);
                logger.LogDebug("Wylogowanie", new[] { exc.Message });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

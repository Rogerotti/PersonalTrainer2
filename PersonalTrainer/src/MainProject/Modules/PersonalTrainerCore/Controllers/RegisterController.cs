using Framework.Models.Dto;
using Framework.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTrainerCore.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserManagement userManagement;
        private readonly ILogger<LoginController> logger;

        public RegisterController(
            IUserManagement userManagement,
            ILogger<LoginController> logger)
        {
            this.userManagement = userManagement;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserDto user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("context", "application-json");
                    client.DefaultRequestHeaders.Add("secure-token", "TODOD z sesji");

                    client.BaseAddress = new Uri("https://localhost:44323/api/user/register");


 
                    // Serialize our concrete class into a JSON String
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(user));

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    var r = await client.PostAsync("https://localhost:44323/api/user/register", httpContent);

                    userManagement.RegisterUser(user.Login, user.Email, user.Password, user.Gender, user.Height, user.Weight, user.Age);
                    userManagement.Login(user.Login, user.Password);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(user);
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("AdditionalValidation", exc.Message);
                logger.LogDebug("rejestracja nie powiodła się.", new[] { exc.Message });
                return View(user);
            }

        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new UserDto());
        }
    }
}

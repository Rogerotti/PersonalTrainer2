using Framework.Models.ApiDto;
using Framework.Models.Dto;
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
        private readonly ILogger<LoginController> logger;

        public RegisterController(ILogger<LoginController> logger)
        {
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
                    var payload = new UserApiDto()
                    {
                        Username = user.Login,
                        Password = user.Password,
                        Weight = user.Weight,
                        Height = user.Height,
                        UserState = (Int32)user.UserState,
                        Administrator = false,
                        Age = user.Age,
                        Email = user.Email,
                        Gender = user.Gender,
                    };

                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("https://localhost:44323/api/user/register", httpContent);

                    client = new HttpClient();
                    var payload2 = new UserSimpleApiDto() { Username = user.Login, Password = user.Password };
                    var stringPayload2 = await Task.Run(() => JsonConvert.SerializeObject(payload2));
                    var httpContent2 = new StringContent(stringPayload2, Encoding.UTF8, "application/json");
                    var result2 = await client.PostAsync("https://localhost:44323/api/user/login", httpContent);
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

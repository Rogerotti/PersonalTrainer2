using Framework.Models;
using Framework.Models.ApiDto;
using Framework.Models.Dto;
using Framework.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PersonalTrainerCore.Api;
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
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> Index(UserDto user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    /*
                    HttpClient recaptchaClient = new HttpClient();
                    var secret = "6LeBIUAUAAAAAKrLXCvJLqfF1vf6_RX8rk40-9KS";
                    var recaptchaPayload = new RecaptchaDto() { Secret = secret, Response = response };
                    var recaptchaStringPayload = await Task.Run(() => JsonConvert.SerializeObject(recaptchaPayload));
                    var recaptchaHttpContent = new StringContent(recaptchaStringPayload, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var recaptchaResult = await recaptchaClient.PostAsync(ApiUrls.reCaptchaUrl, recaptchaHttpContent);
                    var rr = await recaptchaResult.Content.ReadAsStringAsync();
                    RecaptchaResult res2 = JsonConvert.DeserializeObject(await recaptchaResult.Content.ReadAsStringAsync(), typeof(RecaptchaResult)) as RecaptchaResult;
                    */
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
                        Gender = (user.Gender == null) ? 0 : (Int32)user.Gender,
                    };

                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(ApiUrls.RegisterUrl, httpContent);

                    client = new HttpClient();
                    var payload2 = new UserSimpleApiDto() { Username = user.Login, Password = user.Password };
                    var stringPayload2 = await Task.Run(() => JsonConvert.SerializeObject(payload2));
                    var httpContent2 = new StringContent(stringPayload2, Encoding.UTF8, "application/json");
                    var result2 = await client.PostAsync(ApiUrls.LoginUrl, httpContent);
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
            return View(new UserDto() {Gender = null });
        }
    }
}

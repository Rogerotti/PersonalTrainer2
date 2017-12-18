using Framework.Models.ApiDto;
using Framework.Models.Dto;
using Framework.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PersonalTrainerCore.Api;
using PersonalTrainerCore.Api.Dto;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTrainerCore.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserManagement userManagement;
        private readonly ISession session;
        private readonly ILogger<LoginController> logger;

        public LoginController(
            IUserManagement userManagement,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LoginController> logger)
        {
            this.userManagement = userManagement;
            this.logger = logger;
            session = httpContextAccessor.HttpContext.Session;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new UserDto());
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserDto user)
        {
            try
            {
                var client = new HttpClient();
                var payload = new UserSimpleApiDto() { Username = user.Login, Password = user.Password };
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(ApiUrls.LoginUrl, httpContent);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SessionDto res = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(SessionDto)) as SessionDto;
                    session.Clear();
                    session.SetString("userId", res.UserId.ToString());
                    session.SetString("session-token", res.Token);
                    session.SetString("username", res.Username);
                    session.SetString("IsAdmin", res.Admin.ToString());
                }

            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
                logger.LogDebug("Logowanie przez użytkownika", new[] { exc.Message });
                session.Clear();
                return View(user);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AdminLogin()
        {
            try
            {
                var client = new HttpClient();
                var payload = new UserSimpleApiDto() { Username = "Rogerottii", Password = "Roger!994" };
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var result = await client.PostAsync(ApiUrls.LoginUrl, httpContent);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SessionDto res = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(SessionDto)) as SessionDto;
                    session.Clear();
                    session.SetString("userId", res.UserId.ToString());
                    session.SetString("session-token", res.Token);
                    session.SetString("username", res.Username);
                    session.SetString("IsAdmin", res.Admin.ToString());
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                var user = new UserDto()
                {
                    Login = "Rogerottii",
                    Password = "Roger!994",
                    Email = "bb@gmail.com",
                    Age = 22,
                    Gender = 0,
                    Height = 160,
                    Weight = 50,
                    IsAdministrator = true,
                };

                try
                {
                    var client = new HttpClient();
                    var payload = new UserSimpleApiDto() { Username = user.Login, Password = user.Password };
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(ApiUrls.LoginUrl, httpContent);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SessionDto res = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(SessionDto)) as SessionDto;
                        session.Clear();
                        session.SetString("userId", res.UserId.ToString());
                        session.SetString("session-token", res.Token);
                        session.SetString("username", res.Username);
                        session.SetString("IsAdmin", res.Admin.ToString());
                    }

                }
                catch (Exception exc)
                {
                    ModelState.TryAddModelError("AdditionalValidation", exc.Message);
                    logger.LogDebug("Logowanie przez użytkownika", new[] { exc.Message });
                    session.Clear();
                    return View(user);
                }

                return RedirectToAction("Index", "Home");
            }
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

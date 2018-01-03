using Framework.Models.ApiDto;
using Framework.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PersonalTrainerCore.Api;
using PersonalTrainerCore.Api.Dto;
using PersonalTrainerCore.Session;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTrainerCore.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<LoginController> logger;

        public LoginController(
            IHttpContextAccessor httpContextAccessor,
            ILogger<LoginController> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
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
                var session = httpContextAccessor.HttpContext.Session;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SessionDto res = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(SessionDto)) as SessionDto;
                    session.Clear();
                    session.SetString("userId", res.UserId.ToString());
                    session.SetString("session-token", res.Token);
                    session.SetString("username", res.Username);
                    session.SetString("IsAdmin", res.Admin.ToString());
                }
                else
                {
                    var message = await result.Content.ReadAsStringAsync();
                    ModelState.TryAddModelError("AdditionalValidation", message);
                    logger.LogDebug("Logowanie przez użytkownika", new[] { message });
                    httpContextAccessor.HttpContext.Session.Clear();
                    session.Clear();
                    return View(user);
                }

            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
                logger.LogDebug("Logowanie przez użytkownika", new[] { exc.Message });
                httpContextAccessor.HttpContext.Session.Clear();
                return View(user);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AdminLogin()
        {
            var client = new HttpClient();
            var result = await client.GetAsync(ApiUrls.LoginAdminUrl);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SessionDto res = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(SessionDto)) as SessionDto;
                var session = httpContextAccessor.HttpContext.Session;
                session.Clear();
                session.SetString(SessionTypes.UserId, res.UserId.ToString());
                session.SetString(SessionTypes.Token, res.Token);
                session.SetString(SessionTypes.Username, res.Username);
                session.SetString(SessionTypes.IsAdmin, res.Admin.ToString());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            try
            {
                httpContextAccessor.HttpContext.Session.Clear();
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

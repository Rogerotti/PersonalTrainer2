using Framework.Models;
using Framework.Models.ApiDto;
using Framework.Models.Dto;
using Framework.Services;
using Framework.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersonalTrainerCore.Api;
using PersonalTrainerCore.Session;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PersonalTrainerControlPanel.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserManagement userManagement;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IProductManagement productManagement;

        public AdminController(
            IUserManagement userManagement,
            IHttpContextAccessor httpContextAccessor,
            IProductManagement productManagement)
        {
            this.userManagement = userManagement;
            this.productManagement = productManagement;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [RestoreModelStateFromTempData]
        public async Task<IActionResult> Users()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                var url = ApiUrls.GetUsers;
                var result = await client.GetAsync(url);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                    return View();
                }

                var users = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(IEnumerable<UserApiDto>)) as IEnumerable<UserApiDto>;
                List<UserDto> list = new List<UserDto>();
                foreach (var item in users)
                {
                    list.Add(new UserDto()
                    {
                        Age = item.Age,
                        Email = item.Email,
                        Gender = item.Gender,
                        Height = item.Height,
                        IsAdministrator = item.Administrator,
                        Weight = item.Weight,
                        UserState = (UserState)item.UserState,
                        Login = item.Username,
                        UserId = item.Id,
                    });
                }
                return View(list);
            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AcceptSubscription(String productId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                var url = ApiUrls.AcceptSubscriptionProduct.Replace("#ID#", productId);
                var result = await client.PostAsync(url, new StringContent(""));

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                    return View();
                }
            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
            }

            return RedirectToAction("Products", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> DeclineSubscription(String productId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.DeclineSubscriptionProduct.Replace("#ID#", productId);
            var result = await client.PostAsync(url, new StringContent(""));

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                return View();
            }

            return RedirectToAction("Products", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> SubscribeProduct(String productId)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.SubscribeProduct.Replace("#ID#", productId);
            var result = await client.PostAsync(url, new StringContent(""));

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                return View();
            }

            return RedirectToAction("Products", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> PromoteToAdmin(String userId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                var url = ApiUrls.PromoteUser.Replace("#ID#", userId);
                var result = await client.PostAsync(url, new StringContent(""));

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.TryAddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                }
            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
            }
            return RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> DegradateUser(String userId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                var url = ApiUrls.DegradateUser.Replace("#ID#", userId);
                var result = await client.PostAsync(url, new StringContent(""));

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.TryAddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                }
            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
            }
            return RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        [SetTempDataModelState]
        public async Task<IActionResult> DeleteUser(String userId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                var url = ApiUrls.DeleteUser.Replace("#ID#", userId);
                var result = await client.DeleteAsync(url);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.TryAddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                }
            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
            }

            return RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                var url = ApiUrls.GetProductsUrl;
                var result = await client.GetAsync(url);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                   var productList = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(IEnumerable<ProductDto>)) as IEnumerable<ProductDto>;
                    return View(productList);
                }
            }
            catch (Exception exc)
            {
                ModelState.TryAddModelError("AdditionalValidation", exc.Message);
            }
            return View(new List<ProductDto>());
        }

        [HttpPost]
        public async Task<JsonResult> GetUserDetails([FromBody]JToken jsonBody)
        {
            var id = jsonBody.Value<String>("Id");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.GetUser.Replace("#ID#", id);
            var result = await client.GetAsync(url);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UserApiDto apiUserDto = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(UserApiDto)) as UserApiDto;
                var userDto = new UserDto()
                {
                    Age = apiUserDto.Age,
                    Email = apiUserDto.Email,
                    Gender = apiUserDto.Gender,
                    Height = apiUserDto.Height,
                    IsAdministrator = apiUserDto.Administrator,
                    Weight = apiUserDto.Weight,
                    UserState = (UserState)apiUserDto.UserState,
                    Login = apiUserDto.Username,
                    UserId = apiUserDto.Id,
                };
                return new JsonResult(userDto);
            }

            return new JsonResult(new UserDto());
        }
    }
}

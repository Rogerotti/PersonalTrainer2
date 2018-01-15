using Framework.Extensions;
using Framework.Models;
using Framework.Models.Dto;
using Framework.Models.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersonalTrainerCore.Api;
using PersonalTrainerCore.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTrainerDiet.Controllers
{
    public class DietController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private const String additionalMealsId = nameof(additionalMealsId);
        private const String productGuidId = nameof(productGuidId);
        private const String productQuantityId = nameof(productQuantityId);
        private const String productMealTypeId = nameof(productMealTypeId);
        private const String mealTypeId = nameof(mealTypeId);

        public DietController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Day(DateTime id)
        {
            DailyFoodDto ProductDto = null;
            DateTime dayDate;
            if (default(DateTime).Equals(id))
            {
                dayDate = DateTime.Today;
            }
            else
                dayDate = id;

            var additionalMeal = TempData[additionalMealsId] as Boolean?;
            if (additionalMeal == null ? false : (Boolean)additionalMeal)
            {
                var guids = TempData[productGuidId] as IEnumerable<Guid>;
                var quants = TempData[productQuantityId] as IEnumerable<Int32>;
                var mealType = TempData[productMealTypeId] as IEnumerable<Int32>;
                var enumMealType = TempData[mealTypeId] as Int32?;
                if (guids != null && guids.Count() != 0)
                {
                    List<DailyFoodProductDto> lista = new List<DailyFoodProductDto>();
                    var a = guids.ToArray();
                    var b = quants.ToArray();
                    var c = mealType.ToArray();
                    for (int i = 0; i < a.Length; i++)
                    {
                        lista.Add(new DailyFoodProductDto()
                        {
                            ProductId = a[i],
                            ProductQuantity = b[i],
                            MealType = (MealType)c[i]
                        });
                    }

                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));
                    var uri = ApiUrls.GetDayMeals.Replace("#DATE#", dayDate.ToString("yyyy-MM-ddTHH:mm:ss"));
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(lista));
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    var result2 = await httpClient.PostAsync(uri, httpContent);
                    if (result2.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ProductDto = JsonConvert.DeserializeObject(await result2.Content.ReadAsStringAsync(), typeof(DailyFoodDto)) as DailyFoodDto;
                    }
                }
            }
            else {
                var guids = TempData[productGuidId] as IEnumerable<Guid>;
                var quants = TempData[productQuantityId] as IEnumerable<Int32>;
                var mealType = TempData[productMealTypeId] as IEnumerable<Int32>;
                var enumMealType = TempData[mealTypeId] as Int32?;
            }

            if (ProductDto == null)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                    var session = httpContextAccessor.HttpContext.Session;
                    var userId = session.GetString(SessionTypes.UserId);
                    var urlTemplate = ApiUrls.GetDayMeal.Replace("#USERID#", userId).Replace("#ID#", dayDate.ToString("yyyy-MM-ddTHH:mm:ss"));

                    var res = await httpClient.GetAsync(urlTemplate);
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ProductDto = JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync(), typeof(DailyFoodDto)) as DailyFoodDto;
                    }
                }
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.UserGoalsUrl.Replace("#ID#", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.UserId));
            var result = await client.GetAsync(url);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UserGoalsDto userGoals = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(UserGoalsDto)) as UserGoalsDto;
                var dayView = new DayView()
                {
                    DayProteins = ProductDto.DayProteins,
                    DayFat = ProductDto.DayFat,
                    DailyProduct = ProductDto.DailyProduct,
                    DayCarbohydrates = ProductDto.DayCarbohydrates,
                    Day = ProductDto.Day,
                    DayCalories = ProductDto.DayCalories,
                    AvaibleCalories = userGoals.Calories,
                    AvaibleCarbohydrates = userGoals.Carbohydrates,
                    AvaibleFat = userGoals.Fat,
                    AvaibleProteins = userGoals.Proteins,
                };
                client.Dispose();
                return View(dayView);
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return RedirectToAction("UserGoals", "Diet");

            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Day(IEnumerable<Guid> productId, IEnumerable<Int32> quantity, IEnumerable<Int32> productMealType, Int32 buttonType, DateTime Day)
        {
            // TODO dodawanie
            if (buttonType == 4)
            {
                var idList = productId.ToList();
                var quantityList = quantity.ToList();
                var mealTypeList = productMealType.ToList();
                List<DailyFoodProductDto> food = new List<DailyFoodProductDto>();
                for (int i = 0; i < productId.Count(); i++)
                {
                    food.Add(new DailyFoodProductDto()
                    {
                        ProductId = idList[i],
                        ProductQuantity = quantityList[i],
                        MealType = (MealType)mealTypeList[i]
                    });
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));
                var url = ApiUrls.SubmitDayMeal
                    .Replace("#USERID#", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.UserId))
                    .Replace("#DATE#", Day.ToString("yyyy-MM-ddTHH:mm:ss"));

                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(food));
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(url, httpContent);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.TryAddModelError("AdditionalValidation", "Błąd dodawania dnia żywieniowego");
                }

                return RedirectToAction("Day", "Diet");
            }


            TempData[productGuidId] = productId.ToList();
            TempData[productQuantityId] = quantity.ToList();
            TempData[productMealTypeId] = productMealType.ToList();
            TempData[mealTypeId] = buttonType;

            return RedirectToAction("AddFood", "Diet", new { dateTime = Day });
        }

        [HttpGet]
        public async Task<IActionResult> AddFood(DateTime dateTime)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var result = await client.GetAsync(ApiUrls.GetProductsUrl);

            var allProducts = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(IEnumerable<ProductDto>)) as IEnumerable<ProductDto>;
            var userId = httpContextAccessor.HttpContext.Session.GetString(SessionTypes.UserId);
            var userProducts = allProducts
                .Select(x => x)
                .Where(y => y.UserId.Equals(new Guid(userId)));

            var dto = new SearchProductsDto()
            {
                AllProducts = allProducts.ToList(),
                UserProducts = userProducts.ToList(),
                RecentProducts = userProducts.ToList(),
                Day = dateTime
            };
            return View(dto);
        }

        [HttpPost]
        public IActionResult AddFood(List<Guid> ids, List<Int32> quantity, DateTime Day, Boolean[] tess)
        {
            List<Guid> properIds = new List<Guid>();
            List<Int32> properQuantities = new List<int>();

            for (int i = 0; i < tess.Count(); i++)
            {
                if (tess[i])
                {
                    properIds.Add(ids[i]);
                    properQuantities.Add(quantity[i]);
                }
            }

            List<Int32> properMealTypes = new List<int>();
            var enumMealType = TempData[mealTypeId] as Int32?;

            for (int i = 0; i < properIds.Count(); i++)
                properMealTypes.Add((Int32)enumMealType);

            var guids = TempData[productGuidId] as IEnumerable<Guid>;
            if (guids != null && guids.Count() != 0)
            {
                var quants = TempData[productQuantityId] as IEnumerable<Int32>;
                var mealType = TempData[productMealTypeId] as IEnumerable<Int32>;
                properIds.AddRange(guids);
                properQuantities.AddRange(quants);
                properMealTypes.AddRange(mealType);
            }

            TempData[additionalMealsId] = true;
            TempData[productGuidId] = properIds;
            TempData[productQuantityId] = properQuantities;
            TempData[productMealTypeId] = properMealTypes;
            TempData[mealTypeId] = enumMealType;

            return RedirectToAction("Day", "Diet", new { id = Day.ToString("yyyy-MM-dd")} );
        }

        /// <summary>
        /// Odpowiedzialny za wczytanie formatki dodania i edycji produktu użytkownika.
        /// </summary>
        /// <param name="productId">Id produku. W przypadku dodawania nowego produktu przyjmuje wartość pustą lub null.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddEditProduct(String productId)
        {
            var dto = new AddEditProductView() { Macro = new Macro() };

            if (String.IsNullOrWhiteSpace(productId))
            {
                dto.Macro.Calories = 0;
                dto.Macro.Carbohydrates = 0;
                dto.Macro.Fat = 0;
                dto.Macro.Protein = 0;
                dto.Macro.Quantity = 0;
                dto.Macro.QuantityType = QuantityType.Grams;
                dto.Mode = Mode.Add;
                ViewData["Title"] = "Add";
            }
            else
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));
                var url = ApiUrls.GetProductUrl.Replace("#ID#", productId);
                var result = await client.GetAsync(url);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.TryAddModelError("AdditionalValidation", "Błąd Edycji produktu");
                }
                else
                {
                    var product = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(ProductDto)) as ProductDto;
                    dto.Name = product.Name;
                    dto.Manufacturer = product.Manufacturer;
                    dto.ProductId = product.ProductId;
                    dto.Type = product.Type;
                    dto.TypeDisplayName = product.Type.GetDisplayName();
                    dto.Macro.Calories = product.Macro.Calories;
                    dto.Macro.Carbohydrates = product.Macro.Carbohydrates;
                    dto.Macro.Fat = product.Macro.Fat;
                    dto.Macro.Protein = product.Macro.Protein;
                    dto.Macro.Quantity = product.Macro.Quantity;
                    dto.Macro.QuantityType = product.Macro.QuantityType;
                    dto.Mode = Mode.Edit;
                    ViewData["Title"] = "Edit";
                }
            }

            return View(dto);

        }

        /// <summary>
        /// Odpowiedzialny za zapis nowo dodanego lub edytowanego produktu użytkownika.
        /// </summary>
        /// <param name="dto">Dto produktu z widoku. <see cref="AddEditProductView"/></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddEditProduct(AddEditProductView dto)
        {
            try
            {
                var payload = new ProductDto()
                {
                    Macro = dto.Macro,
                    Name = dto.Name,
                    Manufacturer = dto.Manufacturer,
                    Type = dto.Type,
                    TypeDisplayName = dto.Type.GetDisplayName()
                };

                if (dto.Mode == Mode.Add)
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));
                    payload.UserId = new Guid(httpContextAccessor.HttpContext.Session.GetString(SessionTypes.UserId));
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    var result = await client.PostAsync(ApiUrls.AddProductUrl, httpContent);
                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        ModelState.AddModelError("AdditionalValidation", await result.RequestMessage.Content.ReadAsStringAsync());
                        return View(dto);
                    }
                }
                else if (dto.Mode == Mode.Edit)
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                    payload.ProductId = dto.ProductId;
                    var url = ApiUrls.EditProductUrl.Replace("#ID#", dto.ProductId.ToString());
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    var result = await client.PutAsync(url, httpContent);
                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        ModelState.AddModelError("AdditionalValidation", await result.RequestMessage.Content.ReadAsStringAsync());
                        return View(dto);
                    }
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("AdditionalValidation", exc.Message);
                return View(dto);
            }

            return RedirectToAction("ProductList", "Diet");
        }

        /// <summary>
        /// Odpowiedzialny za usunięcie produktu użytkownika.
        /// </summary>
        /// <param name="productDeleteId">Id produktu.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(String productDeleteId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.DeleteProduct.Replace("#ID#", productDeleteId);
            var result = await client.DeleteAsync(url);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                return View();
            }
            return RedirectToAction("ProductList", "Diet");
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeProduct(String productSubscribeId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.SubscribeProduct.Replace("#ID#", productSubscribeId);
            var result = await client.PostAsync(url, new StringContent(""));

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                return View();
            }

            return RedirectToAction("ProductList", "Diet");
        }

        [HttpPost]
        public async Task<IActionResult> CancelSubscription(String productCancelSubscribeId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.CancelSubscriptionProduct.Replace("#ID#", productCancelSubscribeId);
            var result = await client.PostAsync(url, new StringContent(""));

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError("AdditionalValidation", await result.Content.ReadAsStringAsync());
                return View();
            }

            return RedirectToAction("ProductList", "Diet");
        }

        [HttpGet]
        public async Task<IActionResult> UserGoals()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.UserGoalsUrl.Replace("#ID#", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.UserId));
            var result = await client.GetAsync(url);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UserGoalsDto goals = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(UserGoalsDto)) as UserGoalsDto;
                if (goals.Calories == 0)
                    goals.Calories = 1;

                var view = new UserGoalsView()
                {
                    Calories = goals.Calories,
                    Carbohydrates = goals.Carbohydrates,
                    Fat = goals.Fat,
                    Proteins = goals.Proteins,
                    UserId = goals.UserId,
                    PercentageCarbs = goals.PercentageCarbs,
                    PercentageFat = goals.PercentageFat,
                    PercentageProtein = goals.PercentageProtein
                };

                return View(view);
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                return View(new UserGoalsView());
            else
            {
                ModelState.AddModelError("AdditionalValidation", await result.RequestMessage.Content.ReadAsStringAsync());
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserGoals(UserGoalsView dto)
        {
            if (dto != null)
            {
                try
                {
                    var payload = new UserGoalsDto()
                    {
                        UserId = dto.UserId,
                        Calories = dto.Calories,
                        Carbohydrates = dto.Carbohydrates,
                        Fat = dto.Fat,
                        Proteins = dto.Proteins,
                        PercentageCarbs = dto.PercentageCarbs,
                        PercentageFat = dto.PercentageFat,
                        PercentageProtein = dto.PercentageProtein
                    };

                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

                    var url = ApiUrls.UserGoalsUrl.Replace("#ID#", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.UserId));
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    var result = await client.PostAsync(url, httpContent);
                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        ModelState.AddModelError("AdditionalValidation", await result.RequestMessage.Content.ReadAsStringAsync());
                        return View();
                    }
                }
                catch (Exception exc)
                {
                    ModelState.TryAddModelError("AdditionalValidation", exc.Message);
                }
            }

            return View(dto);
        }

        /// <summary>
        /// Odpowiedzialny za wyświetlanie listy produktów.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.GetUserProductsUrl.Replace("#USERID#", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.UserId));
            var result = await client.GetAsync(url);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError("AdditionalValidation", await result.RequestMessage.Content.ReadAsStringAsync());
                return View();
            }

            IEnumerable<ProductDto> products = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(IEnumerable<ProductDto>)) as IEnumerable<ProductDto>;

            var dto = new ProductListDto()
            {
                ProductList = products,
                SelectedProduct = null
            };

            return View(dto);
        }

        /// <summary>
        /// Pozyskuje informacje dotyczące zaznaczonego produktu.
        /// </summary>
        /// <param name="jsonBody"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetProductDetails([FromBody]JToken jsonBody)
        {
            var id = jsonBody.Value<String>("Id");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer \"{0}\"", httpContextAccessor.HttpContext.Session.GetString(SessionTypes.Token)));

            var url = ApiUrls.GetProductUrl.Replace("#ID#", id);
            var result = await client.GetAsync(url);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ProductDto res = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(ProductDto)) as ProductDto;
                return new JsonResult(res);
            }

            return new JsonResult(new ProductDto());
        }

    }
}

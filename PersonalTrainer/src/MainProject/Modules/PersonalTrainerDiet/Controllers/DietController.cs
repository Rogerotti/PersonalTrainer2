﻿using Framework.Extensions;
using Framework.Models;
using Framework.Models.Dto;
using Framework.Models.View;
using Framework.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalTrainerDiet.Controllers
{
    public class DietController : Controller
    {
        private readonly IProductManagement productManagement;
        private readonly IUserGoalsManagement userGoalsManamgenet;

        private const String additionalMealsId = nameof(additionalMealsId);
        private const String productGuidId = nameof(productGuidId);
        private const String productQuantityId = nameof(productQuantityId);
        private const String productMealTypeId = nameof(productMealTypeId);
        private const String mealTypeId = nameof(mealTypeId);

        public DietController(IProductManagement productManagement,
            IUserGoalsManagement userGoalsManamgenet)
        {
            this.productManagement = productManagement;
            this.userGoalsManamgenet = userGoalsManamgenet;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Day(String Id)
        {
            DailyFoodDto ProductDto = null;
            DateTime dayDate;
            if (Id != null)
            {
                dayDate = DateTime.ParseExact(Id, "dd-MM-yyyy",
                                            System.Globalization.CultureInfo.InvariantCulture);
            }
            else
                dayDate = DateTime.Today;

            var additionalMeal = TempData[additionalMealsId] as Boolean?;
            if (additionalMeal == null ? false : (Boolean)additionalMeal )
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

                    ProductDto = productManagement.GetDailyFoodFromDailyFoodProductDto(dayDate, lista);
                }
            }

            if (ProductDto == null)
                ProductDto = productManagement.GetDailyFood(dayDate);


            var userGoals = userGoalsManamgenet.GetCurrentUserGoals();

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

            return View(dayView);
        }

        [HttpPost]
        public IActionResult Day(IEnumerable<Guid> productId, IEnumerable<Int32> quantity, IEnumerable<Int32> productMealType, Int32 buttonType, DateTime Day)
        {
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
                productManagement.SubmitDailyFood(Day, food);

                return RedirectToAction("Day", "Diet");
            }


            TempData[productGuidId] = productId.ToList();
            TempData[productQuantityId] = quantity.ToList();
            TempData[productMealTypeId] = productMealType.ToList();
            TempData[mealTypeId] = buttonType;

            return RedirectToAction("AddFood", "Diet", new { dateTime = Day });
        }

        [HttpGet]
        public IActionResult AddFood(DateTime dateTime)
        {
            var allProducts = productManagement.GetProducts();
            var userProducts = productManagement.GetUserProducts();
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
        public JsonResult GetDayByDate([FromBody]JToken jsonBody)
        {
            var dateString = jsonBody.Value<String>("Date");
            var date = DateTime.Parse(dateString);
            date = date.AddDays(1);

            var food = productManagement.GetDailyFood(date);

            return new JsonResult(food.DailyProduct);
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

            String id = String.Empty;
            String month = String.Empty;
            if (Day.Month < 10)
                month = String.Format("0{0}", Day.Month);
            else
                month = Day.Month.ToString();
            id = String.Format("{0}-{1}-{2}", Day.Day.ToString(), month, Day.Year.ToString());
            return RedirectToAction("Day", "Diet", new { id = id} );
        }

        /// <summary>
        /// Odpowiedzialny za wczytanie formatki dodania i edycji produktu użytkownika.
        /// </summary>
        /// <param name="productId">Id produku. W przypadku dodawania nowego produktu przyjmuje wartość pustą lub null.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddEditProduct(String productId)
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
                var product = productManagement.GetProduct(new Guid(productId));
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

            return View(dto);

        }

        /// <summary>
        /// Odpowiedzialny za zapis nowo dodanego lub edytowanego produktu użytkownika.
        /// </summary>
        /// <param name="dto">Dto produktu z widoku. <see cref="AddEditProductView"/></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddEditProduct(AddEditProductView dto)
        {
            try
            {
                var product = new ProductDto()
                {
                    Macro = dto.Macro,
                    Name = dto.Name,
                    Manufacturer = dto.Manufacturer,
                    Type = dto.Type,
                    TypeDisplayName = dto.Type.GetDisplayName()
                };

                if (dto.Mode == Mode.Add)
                    productManagement.AddProduct(product);
                else if (dto.Mode == Mode.Edit)
                {
                    product.ProductId = dto.ProductId;
                    productManagement.UpdateProduct(product);
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
        public IActionResult DeleteProduct(String productDeleteId)
        {
            productManagement.RemoveProduct(new Guid(productDeleteId));
            return RedirectToAction("ProductList", "Diet");
        }

        [HttpPost]
        public IActionResult SubscribeProduct(String productSubscribeId)
        {
            productManagement.SubscribeProduct(new Guid(productSubscribeId));
            return RedirectToAction("ProductList", "Diet");
        }

        [HttpPost]
        public IActionResult CancelSubscription(String productCancelSubscribeId)
        {
            productManagement.CancelSubscription(new Guid(productCancelSubscribeId));
            return RedirectToAction("ProductList", "Diet");
        }

        [HttpGet]
        public IActionResult UserGoals()
        {
            var goals = userGoalsManamgenet.GetCurrentUserGoals();


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

        [HttpPost]
        public IActionResult UserGoals(UserGoalsView dto)
        {
            if (dto != null)
            {
                try
                {
                    var userGoals = new UserGoalsDto()
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

                    userGoalsManamgenet.SetGoals(userGoals);
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
        public IActionResult ProductList()
        {
            var products = productManagement.GetUserProducts();
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
        public JsonResult GetProductDetails([FromBody]JToken jsonBody)
        {
            var id = jsonBody.Value<String>("Id");
            var productDetails = productManagement.GetProduct(new Guid(id));
            return new JsonResult(productDetails);
        }
    }
}

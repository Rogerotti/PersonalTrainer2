using Microsoft.AspNetCore.Mvc;
using PersonalTrainerApi.Model.Dto.Product;
using PersonalTrainerApi.Services.Products;
using System;
using System.Collections.Generic;

namespace PersonalTrainerApi.Controllers
{
    public class MealController : AuthorizedController
    {
        private readonly IProductManagement productManagement;

        public MealController(IProductManagement productManagement)
        {
            this.productManagement = productManagement;
        }

        /// <summary>
        /// Zwraca produkt o podanym identyfikatorze
        /// GET api/Meal/Get/{id}
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userId}/{date}")]
        [ProducesResponseType(typeof(DailyFoodDto), 200)]
        public IActionResult Get(Guid userId, DateTime date)
        {
            try
            {
                var dailyFood = productManagement.GetDailyFood(userId, date);
                return Ok(dailyFood);
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        [HttpPost("GetDayMeals/{date}")]
        [ProducesResponseType(typeof(DailyFoodDto), 200)]
        public IActionResult GetDayMeals(DateTime date, [FromBody] IEnumerable<DailyFoodProductDto> dto )
        {
            try
            {
                var meals = productManagement.GetDailyFoodFromDailyFoodProductDto(date, dto);
                return Ok(meals);
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        /// <summary>
        /// Zatwierdza dzień
        /// GET api/Meal/SubmitDay/
        /// </summary>
        /// <returns></returns>
        [HttpPost("SubmitDay/{userId}/{date}")]
        [ProducesResponseType(typeof(DailyFoodDto), 200)]
        public IActionResult SubmitDay(Guid userId, DateTime date, [FromBody] IEnumerable<DailyFoodProductDto> products)
        {
            try
            {
                productManagement.SubmitDailyFood(userId, date, products);
                return Ok();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}

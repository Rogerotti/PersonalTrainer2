using Microsoft.AspNetCore.Mvc;
using PersonalTrainerApi.Services;
using System;

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
    }
}

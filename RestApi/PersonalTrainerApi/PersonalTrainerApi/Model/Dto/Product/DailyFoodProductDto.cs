using PersonalTrainerApi.Model.Types;
using System;

namespace PersonalTrainerApi.Model.Dto.Product
{
    public class DailyFoodProductDto
    {
        public Guid ProductId { get; set; }

        public Int32 ProductQuantity { get; set; }

        public MealType MealType { get; set; }
    }
}

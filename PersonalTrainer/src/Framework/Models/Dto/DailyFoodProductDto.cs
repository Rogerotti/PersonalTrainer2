using System;

namespace Framework.Models.Dto
{
    public class DailyFoodProductDto
    {
        public Guid ProductId { get; set; }

        public Int32 ProductQuantity { get; set; }

        public MealType MealType { get; set; }
    }
}

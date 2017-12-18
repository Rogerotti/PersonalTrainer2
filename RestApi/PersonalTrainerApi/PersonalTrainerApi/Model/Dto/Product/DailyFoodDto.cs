using System;
using System.Collections.Generic;

namespace PersonalTrainerApi.Model.Dto.Product
{
    public class DailyFoodDto
    {
        public List<DailyProductDto> DailyProduct { get; set; }

        public DateTime Day { get; set; }

        public Decimal DayCalories { get; set; }

        public Decimal DayProteins { get; set; }

        public Decimal DayFat { get; set; }

        public Decimal DayCarbohydrates { get; set; }
    }
}

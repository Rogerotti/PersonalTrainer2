namespace Framework.Models.Dto
{
    public class DailyProductDto
    {
        public MealType MealType { get; set; }

        public ProductDto Product { get; set; }

        public Macro CurrentMacro { get; set; }
    }
}

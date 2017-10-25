using System;

namespace Framework.Models.Dto
{
    public class UserGoalsDto
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// Białko użytkownika które ma za zadanie zjeść każdnego dnia.
        /// </summary>
        public Int32 Proteins { get; set; }

        /// <summary>
        /// Tłuszcze użytkownika które ma za zadanie zjeść każdnego dnia.
        /// </summary>
        public Int32 Fat { get; set; }

        /// <summary>
        /// Węglowodany użytkownika które ma za zadanie zjeść każdnego dnia.
        /// </summary>
        public Int32 Carbohydrates { get; set; }

        /// <summary>
        /// Kalorie użytkownika które ma za zadanie zjeść każdnego dnia.
        /// </summary>
        public Int32 Calories { get; set; }

        /// <summary>
        /// Procentowe zapotrzebowanie na tłuszcze.
        /// </summary>
        public Int32 PercentageFat { get; set; }

        /// <summary>
        /// Procentowe zapotrzebowanie na białko.
        /// </summary>
        public Int32 PercentageProtein { get; set; }

        /// <summary>
        /// Procentowe zapotrzebowanie na węglowodany.
        /// </summary>
        public Int32 PercentageCarbs { get; set; }
    }
}

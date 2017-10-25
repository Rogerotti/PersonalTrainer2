using System;

namespace Framework.Models.View
{
    /// <summary>
    /// Model widoku dla ustawień celów użytkownika.
    /// </summary>
    public class UserGoalsView
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// Białko w diecie.
        /// </summary>
        public Int32 Proteins { get; set; }

        /// <summary>
        /// Tłuszcz w diecie.
        /// </summary>
        public Int32 Fat { get; set; }

        /// <summary>
        /// Węglowodany w diecie.
        /// </summary>
        public Int32 Carbohydrates { get; set; }

        /// <summary>
        /// Kalorie w diecie.
        /// </summary>
        public Int32 Calories { get; set; }

        /// <summary>
        /// Procentowa ilość tłuszczu w diecie.
        /// </summary>
        public Int32 PercentageFat { get; set; }

        /// <summary>
        /// Procentowa ilość białka w diecie.
        /// </summary>
        public Int32 PercentageProtein { get; set; }

        /// <summary>
        /// Procentowa ilość węglowodanów w diecie.
        /// </summary>
        public Int32 PercentageCarbs { get; set; }
    }
}

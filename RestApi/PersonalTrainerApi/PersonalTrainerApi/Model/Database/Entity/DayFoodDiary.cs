using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalTrainerApi.Model.Database.Entity
{
    /// <summary>
    /// Tabela przedstawiająca dziennik okna żywieniowego użytkowika dla danego dnia.
    /// </summary>
    [Table(nameof(DayFoodDiary))]
    public class DayFoodDiary
    {
        /// <summary>
        /// Id dnia żywieniowego.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid DayId { get; set; }

        /// <summary>
        /// Id użytkownika którego okno żywieniowe dotyczy.
        /// </summary>
        [ForeignKey(nameof(Entity.User))]
        public Guid UserId { get; set; }

        /// <summary>
        /// Data dnia żywienionwego.
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Spożyte kalorie w ciągu dnia.
        /// </summary>
        public Decimal TotalCalories { get; set; }

        /// <summary>
        /// Spożyte białko w ciągu dnia.
        /// </summary>
        public Decimal TotalProteins { get; set; }

        /// <summary>
        /// Spożyty tłuszcz w ciągu dnia.
        /// </summary>
        public Decimal TotalFat { get; set; }


        /// <summary>
        /// Spożyte węglowodany w ciągu dnia.
        /// </summary>
        public Decimal TotalCarbohydrates { get; set; }

        /// <summary>
        /// Użytkownik do którego dany dzień żywieniowy należy.
        /// </summary>
        [Required]
        public virtual User User { get; set; }

        /// <summary>
        /// Lista posiłków spożytych podczas danego dnia żywieniowego.
        /// </summary>
        public virtual ICollection<DiaryProduct> DiaryProducts { get; set; }
    }
}

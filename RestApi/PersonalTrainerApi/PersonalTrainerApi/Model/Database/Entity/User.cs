using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PersonalTrainerApi.Model.Database.Entity
{
    [Table(nameof(User))]
    public class User
    {
        /// <summary>
        /// Id użytkownika.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Nazwa użytkownika
        /// </summary>
        [Required]
        public String UserName { get; set; }

        /// <summary>
        /// Wartość hashowana hasła.
        /// </summary>
        [Required]
        public String HashCode { get; set; }

        /// <summary>
        /// SóL hasła.
        /// </summary>
        [Required]
        public String Salt { get; set; }

        /// <summary>
        /// Adres e-mail użytkownika.
        /// </summary>
        [Required]
        public String Email { get; set; }

        /// <summary>
        /// Flaga decydująca o tym czy użytkownik jest administratorem systemu.
        /// </summary>
        [Required]
        public Boolean Administrator { get; set; }

        /// <summary>
        /// Dane dodatkowe użytkownika.
        /// </summary>
        [Required]
        public virtual UserDetails UserDetails { get; set; }

        /// <summary>
        /// Status użytkownika 
        /// 0 - działa
        /// 1 - zablokowany
        /// 2 - usunięty
        /// </summary>
        public Int32 UserState { get; set; }

        /// <summary>
        /// Lista dni żywieniowych.
        /// </summary>
        public virtual ICollection<DayFoodDiary> DayFoodDiary { get; set; }

        /// <summary>
        /// Lista dodadynch produktów do systemu.
        /// </summary>
        public virtual ICollection<Product> Products { get; set; }
    }
}

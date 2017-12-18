using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalTrainerApi.Model.Database.Entity
{
    /// <summary>
    /// Tabela przedstawiająca produkty dodawane przez użytkowników.
    /// </summary>
    [Table(nameof(Product))]
    public class Product
    {
        /// <summary>
        /// Id produktu.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ProductId { get; set; }

        [Required]
        /// <summary>
        /// Id użytkownika dodającego produkt.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nazwa produktu.
        /// </summary>
        [Required]
        public String Name { get; set; }

        /// <summary>
        /// Nazwa producenta.
        /// </summary>
        public String Manufacturer { get; set; }

        /// <summary>
        /// Typ produktu.
        /// 0 - Nabiał
        /// TODO..
        /// </summary>
        [Required]
        public Int32 ProductType { get; set; }

        /// <summary>
        /// Stan produktu.
        /// 0 - Prywatny produkt użytkownika.
        /// 1 - Produkt zlecony do zatwierdzenia przez administratora systemu.
        /// 2 - Produkt odrzucony przez administratora
        /// 3 - Produkt zaakceptowany przez administratora.
        /// </summary>
        [Required]
        public Int32 ProductState {get;set;}

        [Required]
        public virtual ProductDetails ProductDetails { get; set; }

        public virtual ICollection<DiaryProduct> DiaryProducts { get; set; }
    }
}

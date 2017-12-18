using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalTrainerApi.Model.Database.Entity
{
    /// <summary>
    /// Tabela przedstawiająca informację dodatkowe produktów.
    /// </summary>
    [Table(nameof(ProductDetails))]
    public class ProductDetails
    {
        /// <summary>
        /// Id produktu.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey(nameof(Entity.Product))]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Ilość białka w produkcie.
        /// </summary>
        [Required]
        public Decimal Protein { get; set; }

        /// <summary>
        /// Ilość tłuszczu w produkcie.
        /// </summary>
        [Required]
        public Decimal Fat { get; set; }

        /// <summary>
        /// Ilość węglowodanów w produkcie.
        /// </summary>
        [Required]
        public Decimal Carbohydrates { get; set; }


        /// <summary>
        /// Ilość kalorii w produkcie.
        /// </summary>
        [Required]
        public Decimal Calories { get; set; }

        /// <summary>
        /// Typ skali produktu.
        /// 0 - gramy
        /// 1 - mililitry
        /// 2 - sztuki
        /// 3 - opakowania
        /// </summary>
        [Required]
        public Int32 QuantityType { get; set; }

        /// <summary>
        /// Ilość podanego produktu.
        /// </summary>
        [Required]
        public Int32 Quantity { get; set; }

        /// <summary>
        /// Produkt.
        /// </summary>
        [Required]
        public virtual Product Product { get; set; }
    }
}

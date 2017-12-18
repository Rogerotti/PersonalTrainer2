using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalTrainerApi.Model.Database.Entity
{
    /// <summary>
    /// Tabela przedstawiająca dane dodatkowe użytkowników.
    /// </summary>
    [Table(nameof(UserDetails))]
    public class UserDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey(nameof(Entity.User))]
        public Guid UserId { get; set; }

        /// <summary>
        /// Płeć
        /// Zgodnie z ISO/IEC 5218
        /// 0 - nie wiadomo.
        /// 1 = meżczyzna.
        /// 2 = kobieta.
        /// 9 = nie zaaplikowano.
        /// </summary>
        public Int32 Gender { get; set; }

        /// <summary>
        /// Waga w kilogramach.
        /// </summary>
        public Decimal Weight { get; set; }

        /// <summary>
        /// Wzrost
        /// </summary>
        public Decimal Height { get; set; }

        /// <summary>
        /// Wiek
        /// </summary>
        public Int32 Age { get; set; }

        /// <summary>
        /// Użytkownik.
        /// </summary>
        [Required]
        public virtual User User { get; set; }
    }
}

using Framework.Resources;
using Framework.ValidationAttributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Models.Dto
{
    public class UserDto
    {
        public Guid UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.LoginRequired))]
        [JsonProperty(nameof(Login))]
        [StringLength(20, 
        MinimumLength = 3, 
        ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.LoginLenght))]
        /// <summary>
        /// Nazwa użytkownika
        /// Minimum - 3 znaki
        /// Maximum - 20 znaków.
        /// </summary>
        public String Login { get; set; }


        [Required(ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.PasswordRequired))]
        [JsonProperty(nameof(Password))]
        [StringLength(20, MinimumLength = 5, 
        ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.PasswordLength))]

        [PasswordRegex(ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.PasswordRegex))]
        /// <summary>
        /// Hasło użytkownika.
        /// </summary>
        public String Password { get; set; }

        [PasswordConfirmation(nameof(Password),
        ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.PasswordNotMatch))]
        public String PasswordConfirmation { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.EmailEmpty))]
        [JsonProperty(nameof(Email))]
        [EmailRegex(ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.EmailRegex))]
        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; set; }

        [JsonProperty(nameof(Gender))]
        /// <summary>
        /// Płeć
        /// Zgodnie z ISO/IEC 5218
        /// 0 - nie wiadomo.
        /// 1 = meżczyzna.
        /// 2 = kobieta.
        /// 9 = nie zaaplikowano.
        /// </summary>
        public Int32 Gender { get; set; }

        [JsonProperty(nameof(Weight))]
        [Required(ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.WeightEmpty))]
        /// <summary>
        /// Waga w kilogramach.
        /// </summary>
        public Decimal Weight { get; set; }


        [Required(ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.HeightEmpty))]
        [JsonProperty(nameof(Height))]
        ///  <summary>
        /// Wzrost w centymetrach.
        /// 1 cm = 0.328 ft
        /// 1 cm = 0.011 yd
        /// </summary>
        public Decimal Height { get; set; }

        /// <summary>
        /// Typ wagi
        /// 
        /// </summary>
        public HeightUnit? HeightUnit { get; set; }

        public UserState UserState { get; set; }

        [Required]
        [Range(8,99,
        ErrorMessageResourceType = typeof(ErrorLanguage),
        ErrorMessageResourceName = nameof(ErrorLanguage.AgeRange))]
        [JsonProperty(nameof(Age))]
        /// <summary>
        /// Wiek.
        /// </summary>
        public Int32 Age { get; set; }

        /// <summary>
        /// Czy użytkownik jest administratorem systemu.
        /// </summary>
        public Boolean IsAdministrator { get; set; }
    }

    public enum HeightUnit {cm,ft,yd };
}

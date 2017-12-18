using System;

namespace Framework.Model
{
    public class UserDto
    {
        public String Id { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }

        public String Email { get; set; }

        public Boolean Administrator { get; set; }

        /// <summary>
        /// Status użytkownika 
        /// 0 - działa
        /// 1 - zablokowany
        /// 2 - usunięty
        /// </summary>
        public Int32 UserState { get; set; }

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
    }
}

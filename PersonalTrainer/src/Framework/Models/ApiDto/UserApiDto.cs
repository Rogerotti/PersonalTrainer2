using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Models.ApiDto
{
    public class UserApiDto
    {
        public Guid Id { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }

        public String Email { get; set; }

        public Boolean Administrator { get; set; }

        public Int32 UserState { get; set; }

        public Int32 Gender { get; set; }

        public Decimal Weight { get; set; }

        public Decimal Height { get; set; }

        public Int32 Age { get; set; }
    }
}

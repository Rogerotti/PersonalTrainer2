using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalTrainerCore.Api.Dto
{
    public class SessionDto
    {
        public string Token { get; set; }

        public string Username { get; set; }

        public bool Admin { get; set; }

        public Guid UserId { get; set; }
    }
}

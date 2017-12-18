using System;

namespace PersonalTrainerApi.Model.Dto.Authorization
{
    public class SessionDto
    {
        public string Token { get; set; }

        public string Username { get; set; }

        public Boolean Admin { get; set; }

        public Guid UserId { get; set; }
    }
}

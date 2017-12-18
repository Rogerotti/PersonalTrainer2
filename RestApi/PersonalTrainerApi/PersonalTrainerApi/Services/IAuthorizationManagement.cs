using System;

namespace PersonalTrainerApi.Services
{
    public interface IAuthorizationManagement
    {
        /// <summary>
        /// Generuje token
        /// </summary>
        /// <param name="username">Nazwa użytkownika</param>
        string GenerateToken(string username);

        Boolean TokenValid(string token);
    }
}

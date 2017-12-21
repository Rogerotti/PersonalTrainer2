using System;

namespace PersonalTrainerApi.Services
{
    public interface IAuthorizationManagement
    {
        /// <summary>
        /// Generuje token
        /// </summary>
        /// <param name="isAdmin">Czy jest administratorem</param>
        string GenerateToken(bool isAdmin = false);
    }
}

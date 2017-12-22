using Framework.Models;
using Framework.Models.Dto;
using System;
using System.Collections.Generic;

namespace Framework.Services
{
    /// <summary>
    /// Manager zarządzająćy użytkownikami.
    /// </summary>
    public interface IUserManagement
    {
        /// <summary>
        /// Zwraca id aktualnie zalogowanego użytkownika.
        /// W przypadku braku zalogowania rzuca wyjątkiem.<see cref="UnauthorizedAccessException"/>
        /// </summary>
        /// <returns></returns>
        Guid GetCurrentUserId();
    }
}

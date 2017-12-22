using PersonalTrainerApi.Model.Dto.User;
using System;

namespace PersonalTrainerApi.Services.Users
{
    /// <summary>
    /// Zarzadza ustawieniami kaloryki użytkownika.
    /// </summary>
    public interface IUserGoalsManagement
    {
        /// <summary>
        /// Pobiera ustawienia dla użytkownika
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <returns></returns>
        UserGoalsDto GetGoals(Guid userId);

        /// <summary>
        /// Ustawia preferowane ustawienia kaloryki dla podanego użytkownika
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <param name="dto"><see cref="UserGoalsDto"/></param>
        void SetGoals(Guid userId, UserGoalsDto dto);
    }
}

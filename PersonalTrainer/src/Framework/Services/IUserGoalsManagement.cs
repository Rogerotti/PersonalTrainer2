using Framework.Models.Dto;
using System;

namespace Framework.Services
{
    public interface IUserGoalsManagement
    {
        void SetGoals(UserGoalsDto userGoals);

        UserGoalsDto GetCurrentUserGoals();

        UserGoalsDto GetUserGoals(Guid userId);
    }
}

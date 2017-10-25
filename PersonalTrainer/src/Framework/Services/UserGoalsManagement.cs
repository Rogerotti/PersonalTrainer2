using System;
using System.Linq;
using Framework.DataBaseContext;
using Framework.Models.Database;
using Framework.Models.Dto;

namespace Framework.Services
{
    public class UserGoalsManagement : IUserGoalsManagement
    {
        private readonly IUserManagement userManagement;
        private readonly DefaultContext context;

        public UserGoalsManagement(
            DefaultContext context,
            IUserManagement userManagement)
        {
            this.context = context;
            this.userManagement = userManagement;
        }

        public void SetGoals(UserGoalsDto dto)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var id = userManagement.GetCurrentUserId();

                var goal = context.UserGoal.FirstOrDefault(x => x.UserId.Equals(id));

                if (goal == null)
                {
                    context.UserGoal.Add(new UserGoal()
                    {
                        UserId = id,
                        Calories = dto.Calories,
                        Carbohydrates = dto.Carbohydrates,
                        Fat = dto.Fat,
                        Proteins = dto.Proteins,
                        PercentageCarbs = dto.PercentageCarbs,
                        PercentageFat = dto.PercentageFat,
                        PercentageProtein = dto.PercentageProtein
                    });

                    context.SaveChanges();
                }
                else
                {
                    goal.Calories = dto.Calories;
                    goal.Carbohydrates = dto.Carbohydrates;
                    goal.Fat = dto.Fat;
                    goal.Proteins = dto.Proteins;
                    goal.PercentageProtein = dto.PercentageProtein;
                    goal.PercentageFat = dto.PercentageFat;
                    goal.PercentageCarbs = dto.PercentageCarbs;

                    context.UserGoal.Update(goal);
                    context.SaveChanges();
                }

                trans.Commit();
            }
        }

        public UserGoalsDto GetCurrentUserGoals()
        {
            var id = userManagement.GetCurrentUserId();
            var goals =  context.UserGoal.FirstOrDefault(x => x.UserId.Equals(id));
            if (goals == null)
            {
                return new UserGoalsDto()
                {
                    UserId = id,
                    Calories = 0,
                    Carbohydrates = 0,
                    Fat = 0,
                    Proteins = 0,
                    PercentageCarbs = 0,
                    PercentageFat = 0,
                    PercentageProtein = 0
                };
            }
        

            return new UserGoalsDto()
            {
                UserId = id,
                Calories = goals.Calories,
                Carbohydrates = goals.Carbohydrates,
                Fat = goals.Fat,
                Proteins = goals.Proteins,
                PercentageCarbs = goals.PercentageCarbs,
                PercentageFat = goals.PercentageFat,
                PercentageProtein = goals.PercentageProtein
            };
        }

        public UserGoalsDto GetUserGoals(Guid userId)
        {
            var goals = context.UserGoal.FirstOrDefault(x => x.UserId.Equals(userId));
            if (goals == null)
                return null;

            return new UserGoalsDto()
            {
                UserId = userId,
                Calories = goals.Calories,
                Carbohydrates = goals.Carbohydrates,
                Fat = goals.Fat,
                Proteins = goals.Proteins,
                PercentageCarbs = goals.PercentageCarbs,
                PercentageFat = goals.PercentageFat,
                PercentageProtein = goals.PercentageProtein
            };
        }
    }
}

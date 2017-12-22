using PersonalTrainerApi.Model.Database.Context;
using PersonalTrainerApi.Model.Database.Entity;
using PersonalTrainerApi.Model.Dto.User;
using System;
using System.Linq;

namespace PersonalTrainerApi.Services.Users
{
    public class UserGoalsManagement : IUserGoalsManagement
    {
        private readonly DefaultContext context;

        public UserGoalsManagement(DefaultContext context)
        {
            this.context = context;
        }

        public UserGoalsDto GetGoals(Guid userId)
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

        public void SetGoals(Guid userId, UserGoalsDto dto)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var goal = context.UserGoal.FirstOrDefault(x => x.UserId.Equals(userId));

                if (goal == null)
                {
                    context.UserGoal.Add(new UserGoal()
                    {
                        UserId = userId,
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

     
    }
}

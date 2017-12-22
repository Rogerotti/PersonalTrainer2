using Framework.DataBaseContext;
using Framework.Models;
using Framework.Models.Database;
using Framework.Models.Dto;
using Framework.Resources;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Services
{
    public class UserManagement : IUserManagement
    {
        private readonly DefaultContext context;
        private readonly ISession session;

        private const String userId = nameof(userId);
        private const String userName = nameof(userName);

        public UserManagement(DefaultContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            session = httpContextAccessor.HttpContext.Session;
        }

        public Guid GetCurrentUserId()
        {
            var id = session.GetString(userId);

            if (String.IsNullOrWhiteSpace(id)) throw new UnauthorizedAccessException(ErrorLanguage.UserNotLoggedIn);

            return new Guid(id);
        }

        private UserDto GetUserDto(User user, UserDetails userDetails)
        {
            return new UserDto()
            {
                UserId = user.UserId,
                UserState = GetUserState(user.UserState),
                Age = userDetails.Age,
                Height = userDetails.Height,
                Login = user.UserName,
                Email = user.Email,
                Password = null,
                PasswordConfirmation = null,
                Weight = userDetails.Weight,
                Gender = userDetails.Gender,
                IsAdministrator = user.Administrator
            };
        }

        private UserState GetUserState(Int32 userState)
        {
            if (userState == 0)
                return UserState.Normal;
            else if (userState == 1)
                return UserState.Blocked;
            else
                return UserState.Deleted;
        }
    }
}

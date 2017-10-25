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

        public void RegisterUser(String username, String email, String password, Int32 gender, Decimal height, Decimal weight, Int32 age)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                CheckUsername(username);
                CheckEmail(email);
                CheckAge(age);
                CheckGender(gender);

                var salt = CreateSalt(32);
                byte[] pass = Encoding.UTF8.GetBytes(password);
                var hash = GenerateSaltedHash(pass, salt);

                Guid userId = Guid.NewGuid();

                var user = new User()
                {
                    UserId = userId,
                    UserState = 0,
                    UserName = username,
                    HashCode = Convert.ToBase64String(hash),
                    Salt = Convert.ToBase64String(salt),
                    Email = email,
                    Administrator = false
                };

                var userDetails = new UserDetails()
                {
                    UserId = userId,
                    Gender = gender,
                    Height = height,
                    Weight = weight,
                    Age = age
                };

                userDetails.User = user;
                context.UsersDetails.Add(userDetails);
                context.SaveChanges();
                trans.Commit();
            }
        }

        public void Login(String username, String password)
        {
            var userList = context.User.ToList();
            if (!userList.Any()) throw new UnauthorizedAccessException("No people in database.");

            var user = userList.FirstOrDefault(x => x.UserName.ToLower().Equals(username.ToLower()));

            if (user == null)
            {
                user = userList.FirstOrDefault(x => x.Email.ToLower().Equals(username.ToLower()));

                if (user == null) throw new UnauthorizedAccessException(ErrorLanguage.UserNameOrPasswordWrong);
            } 
            if(user.UserState == 2) throw new UnauthorizedAccessException(ErrorLanguage.AccountDeleted);

            byte[] salt = Convert.FromBase64String(user.Salt);
            byte[] pass = Encoding.UTF8.GetBytes(password);

            var saltedHash = GenerateSaltedHash(pass, salt);

            var hasCode = Convert.FromBase64String(user.HashCode);
            if (hasCode.SequenceEqual(saltedHash))
            {
                session.SetString(userId, user.UserId.ToString());
                session.SetString(userName, user.UserName);
            }
            else
                throw new UnauthorizedAccessException(ErrorLanguage.UserNameOrPasswordWrong);
        }

        public void DeleteUser(Guid id)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var userDto = context.User.FirstOrDefault(x => x.UserId.Equals(id));
                userDto.UserState = 2;
                context.SaveChanges();
                trans.Commit();
            }
            return;
        }

        public void Logout()
        {
            session.Remove(userId);
            session.Remove(userName);
        }

        public void PromoteToAdmin(Guid userId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var userDto = context.User.FirstOrDefault(x => x.UserId.Equals(userId));
                if (userDto == null) throw new UnauthorizedAccessException(ErrorLanguage.UserNotFound);
                userDto.Administrator = true;
                context.SaveChanges();
                trans.Commit();
            }
        }

        public void DegradateToUser(Guid userId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var userDto = context.User.FirstOrDefault(x => x.UserId.Equals(userId));
                if (userDto == null) throw new UnauthorizedAccessException(ErrorLanguage.UserNotFound);

                userDto.Administrator = false;
                context.SaveChanges();
                trans.Commit();
            }
        }

        public Boolean UserLogedIn()
        {
            var id = session.GetString(userId);
            return String.IsNullOrWhiteSpace(id) ? false : true;
        }

        public Guid GetCurrentUserId()
        {
            var id = session.GetString(userId);

            if (String.IsNullOrWhiteSpace(id)) throw new UnauthorizedAccessException(ErrorLanguage.UserNotLoggedIn);

            return new Guid(id);
        }

        public String GetCurrentUserName()
        {
            var currentUserName = session.GetString(userName);

            if (String.IsNullOrWhiteSpace(currentUserName)) throw new UnauthorizedAccessException(ErrorLanguage.UserNotLoggedIn);

            return currentUserName;
        }

        public UserDto GetCurrentUser()
        {
            Guid userGuid = GetCurrentUserId();

            var userDto = context.User.FirstOrDefault(x => x.UserId.Equals(userGuid));
            var userDetails = context.UsersDetails.FirstOrDefault(x => x.UserId.Equals(userGuid));

            return GetUserDto(userDto, userDetails);
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            var userDto = context.User.Where(x => x.UserState != 2).ToList();
            var userDetails = context.UsersDetails.ToList();

            return GetUserDtoFromParts(userDto, userDetails);
        }

        public IEnumerable<UserDto> GetAllAdministratorUsers()
        {
            var userDto = context.User
                           .Where(x => x.Administrator && x.UserState != 2)
                           .ToList();

            var userDetails = context.UsersDetails.ToList();

            return GetUserDtoFromParts(userDto, userDetails);
        }

        public IEnumerable<UserDto> GetAllNormalsUsers()
        {
            var userDto = context.User
                    .Where(x => !x.Administrator && x.UserState != 2)
                    .ToList();

            var userDetails = context.UsersDetails.ToList();

            return GetUserDtoFromParts(userDto, userDetails);
        }

        public UserDto GetUser(Guid id)
        {
            var userDto = context.User.FirstOrDefault(x => x.UserId.Equals(id));
            var userDetails = context.UsersDetails.FirstOrDefault(x => x.UserId.Equals(id));

            return GetUserDto(userDto, userDetails);
        }

        private IEnumerable<UserDto> GetUserDtoFromParts(List<User> users, List<UserDetails> userDetails)
        {
            List<UserDto> usersDtoList = new List<UserDto>();
            List<KeyValuePair<User, UserDetails>> usersPairList = new List<KeyValuePair<User, UserDetails>>();

            foreach (var item in users)
            {
                var currentUserDetails = userDetails.FirstOrDefault(x => x.UserId.Equals(item.UserId));
                if (currentUserDetails != null)
                    usersPairList.Add(new KeyValuePair<User, UserDetails>(item, currentUserDetails));
            }

            foreach (var item in usersPairList)
                usersDtoList.Add(GetUserDto(item.Key, item.Value));

            return usersDtoList;
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

        /// <summary>
        /// Sprawdza nazwę użytkownika.
        /// Gdy nazwa jest krótsza lub równa 2 znakom lub dłuższa od 20 znaków rzucany jest wyjątek. <see cref="UnauthorizedAccessException"/>
        /// </summary>
        /// <param name="username">Nazwa użytkownika.</param>
        private void CheckUsername(String username)
        {
            if (String.IsNullOrWhiteSpace(username)) throw new UnauthorizedAccessException(ErrorLanguage.UsernameEmpty);
            if (username.Length <= 2 || username.Length > 20) throw new UnauthorizedAccessException(ErrorLanguage.UsernameLength);

            var users = context.User.ToList();
            if (users.Any(x => x.UserName.Equals(username))) throw new UnauthorizedAccessException(ErrorLanguage.UsernameExist);
        }

        /// <summary>
        /// Sprawdza adres email.
        /// </summary>
        /// <param name="email">adres email.</param>
        private void CheckEmail(String email)
        {
            if (String.IsNullOrWhiteSpace(email)) throw new UnauthorizedAccessException(ErrorLanguage.EmailEmpty);

            String emailPatern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            if (!Regex.Match(email, emailPatern).Success) throw new UnauthorizedAccessException(ErrorLanguage.EmailRegex);
        }

        /// <summary>
        /// Sprawdza płeć
        /// Zgodnie z ISO/IEC 5218
        /// 0 - nie wiadomo.
        /// 1 = meżczyzna.
        /// 2 = kobieta.
        /// 9 = nie zaaplikowano.
        /// W przypadku innej wartości rzuca wyjątkiem <see cref="UnauthorizedAccessException"/>
        /// </summary>
        /// <param name="gender">Płeć użytkownika</param>
        private void CheckGender(Int32 gender)
        {
            if (gender != 0 && gender != 1 && gender != 2 && gender != 9)
                throw new UnauthorizedAccessException(ErrorLanguage.GenderError);
        }

        /// <summary>
        /// Sprawdza wiek
        /// od 12 lat do 99 lat.
        /// </summary>
        /// <param name="age">Wiek.</param>
        private void CheckAge(Int32 age)
        {
            if (age > 99 || age < 12) throw new UnauthorizedAccessException(ErrorLanguage.AgeRange);
        }

        /// <summary>
        /// Tworzenie soli
        /// </summary>
        /// <param name="size">Wielkość soli.</param>
        /// <returns></returns>
        private Byte[] CreateSalt(Int32 size)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            Byte[] buff = new Byte[size];

            rng.GetBytes(buff);

            return buff;
        }

        private Byte[] GenerateSaltedHash(Byte[] plainText, Byte[] salt)
        {
            HashAlgorithm algorithm = SHA512.Create();

            Byte[] plainTextWithSaltBytes =
              new Byte[plainText.Length + salt.Length];

            for (Int32 i = 0; i < plainText.Length; i++)
                plainTextWithSaltBytes[i] = plainText[i];

            for (Int32 i = 0; i < salt.Length; i++)
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }


    }
}

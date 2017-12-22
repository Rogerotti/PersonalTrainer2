using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalTrainerApi.Model.Dto.Authorization;
using PersonalTrainerApi.Model.Dto.User;
using PersonalTrainerApi.Services.Authorization;
using PersonalTrainerApi.Services.Users;
using System;
using System.Collections.Generic;

namespace PersonalTrainerApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManagement userManagement;
        private readonly IUserGoalsManagement userGoalsManagement;
        private readonly IAuthorizationManagement authorizationManagement;

        public UserController(
            IUserManagement userManagement,
            IAuthorizationManagement authorizationManagement,
            IUserGoalsManagement userGoalsManagement)
        {
            this.userManagement = userManagement;
            this.authorizationManagement = authorizationManagement;
            this.userGoalsManagement = userGoalsManagement;
        }

        /// <summary>
        /// Rejestracja użytkownika
        /// Post api/user/register
        /// </summary>
        /// <param name="user"><see cref="UserDto"/></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(Guid), 200)]
        [HttpPost(nameof(Register))]
        public IActionResult Register([FromBody] UserDto user)
        {
            try
            {
                var userId = userManagement.RegisterUser(user.Username, user.Password, user.Email, user.Gender, user.Height, user.Weight, user.Age);
                return Ok(userId);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Logowanie użytkownika
        /// Post api/user/register
        /// </summary>
        /// <param name="user"><see cref="SimpleUserDto"/></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(SessionDto), 200)]
        [HttpPost(nameof(Login))]
        public IActionResult Login([FromBody] SimpleUserDto user)
        {
            try
            {
                var session = userManagement.Login(user.Username, user.Password);
                return Ok(session);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Generuje token do testów
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(SessionDto), 200)]
        [HttpGet(nameof(LoginAdminSample))]
        public IActionResult LoginAdminSample()
        {
            try
            {
                var session = userManagement.Login("Rogerottii", "Roger!994");
                return Ok(session);
            }
            catch (Exception)
            {
                userManagement.RegisterUser("Rogerottii", "Roger!994", "admin123@gmail.com", 0, 170, 60, 20);
                var session = userManagement.Login("Rogerottii", "Roger!994");
                return Ok(session);
            }
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(String), 200)]
        [HttpGet(nameof(LoginSample))]
        public IActionResult LoginSample()
        {
            try
            {
                return Ok(authorizationManagement.GenerateToken());
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }


        /// <summary>
        /// Pobiera użytkownika
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <returns></returns>
        [Authorize("admin")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                var dto = userManagement.GetUser(id);
                return Ok(dto);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Usuwa użytkownika
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <returns></returns>
        [Authorize("admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                userManagement.DeleteUser(id);
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [Authorize("admin")]
        [ProducesResponseType(200)]
        [HttpPost("PromoteToAdmin/{id}")]
        public IActionResult PromoteToAdmin(String id)
        {
            try
            {
                userManagement.PromoteToAdmin(new Guid(id));
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [Authorize("admin")]
        [ProducesResponseType(200)]
        [HttpPost("DegradateUser/{id}")]
        public IActionResult DegradateUser(String id)
        {
            try
            {
                userManagement.DegradateToUser(new Guid(id));
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        
        /// <summary>
        /// Pobiera cele użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <returns></returns>
        [Authorize("user")]
        [HttpGet("UserGoal/{id}")]
        [ProducesResponseType(typeof(UserGoalsDto), 200)]
        public IActionResult UserGoal(Guid id)
        {
            try
            {
                var goals = userGoalsManagement.GetGoals(id);
                return Ok(goals);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Ustawia cele użytkownika
        /// </summary>
        /// <param name="id">Identyfikator użytkownika</param>
        /// <param name="dto"><see cref="UserGoalsDto"/></param>
        /// <returns></returns>
        [Authorize("user")]
        [HttpPost("UserGoal/{id}")]
        [ProducesResponseType(200)]
        public IActionResult UserGoal(Guid id, [FromBody] UserGoalsDto dto)
        {
            try
            {
                userGoalsManagement.SetGoals(id, dto);
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }


        [Authorize("admin")]
        [HttpGet(nameof(Users))]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        public IActionResult Users()
        {
            try
            {
                var users = userManagement.GetAllUsers();
                return Ok(users);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

    }
}

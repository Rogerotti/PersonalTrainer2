﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalTrainerApi.Model.Authorization;
using PersonalTrainerApi.Model.Dto.User;
using PersonalTrainerApi.Services;
using System;

namespace PersonalTrainerApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
  
    public class UserController : ControllerBase
    {
        private readonly IUserManagement userManagement;

        public UserController(IUserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        /// <summary>
        /// Rejestracja użytkownika
        /// Post api/user/register
        /// </summary>
        /// <param name="user"><see cref="UserDto"/></param>
        /// <returns></returns>
        [AllowAnonymous]
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
        [HttpPost(nameof(Login))]
        public IActionResult Login([FromBody] SimpleUserDto user)
        {
            try
            {
                var userId = userManagement.Login(user.Username, user.Password);
                return Ok("TOKEN TODO");
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
        [Authorization]
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
    }
}
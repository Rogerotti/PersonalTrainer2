using Framework.Models.Dto;
using Framework.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalTrainerCore.Session;
using System;
using System.Threading.Tasks;

namespace PersonalTrainerCore.Controllers
{
    /// <summary>
    /// Komponent górnej belki.
    /// </summary>
    public class TopMenuViewComponent : ViewComponent
    {
        private readonly IUserManagement userManagement;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TopMenuViewComponent(
            IUserManagement userManagement,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userManagement = userManagement;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(Boolean loggedIn, String username)
        {
            return await Task.Run(() =>
            {
                var session = httpContextAccessor.HttpContext.Session;
                var dto = new TopMenuDto();
                var id = session.GetString(SessionTypes.UserId);
        
                if (id != null)
                {
                    var userName = session.GetString(SessionTypes.Username);
                    var isAdmin = session.GetString(SessionTypes.IsAdmin) == "True" ? true : false;
                    dto.IsLogedIn = true;
                    dto.UserName = userName;
                    dto.IsAdmin = isAdmin;
                }
                else
                {
                    dto.IsLogedIn = false;
                    dto.IsAdmin = false;
                    dto.UserName = String.Empty;
                }

                return View(dto);
            });
        }
    }
}

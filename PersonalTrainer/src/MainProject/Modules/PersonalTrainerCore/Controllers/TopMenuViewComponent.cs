using Framework.Models.Dto;
using Framework.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ISession session;

        public TopMenuViewComponent(
            IUserManagement userManagement,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userManagement = userManagement;
            session = httpContextAccessor.HttpContext.Session;
        }

        public async Task<IViewComponentResult> InvokeAsync(Boolean loggedIn, String username)
        {
            return await Task.Run(() =>
            {
                var dto = new TopMenuDto();
                var id = session.GetString("userId");
        
                if (id != null)
                {
                    var userName = session.GetString("username");
                    var isAdmin = session.GetString("IsAdmin") == "True" ? true : false;
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

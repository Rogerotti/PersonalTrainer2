using Framework.Models.Dto;
using Framework.Services;
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

        public TopMenuViewComponent(IUserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        public async Task<IViewComponentResult> InvokeAsync(Boolean loggedIn, String username)
        {
            return await Task.Run(() =>
            {
                var dto = new TopMenuDto();
                if (userManagement.UserLogedIn())
                {
                    var currentUser = userManagement.GetCurrentUser();
                    dto.IsLogedIn = true;
                    dto.UserName = currentUser.Login;
                    dto.IsAdmin = currentUser.IsAdministrator;
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

using Framework.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalTrainerDiet.Controllers
{
    public class LeftMenuViewComponent : ViewComponent
    {
        private readonly IUserManagement userManagement;

        public LeftMenuViewComponent(IUserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() =>
            {
                var userDto = userManagement.GetCurrentUser();
                return View(userDto);
            });
        }
    }
}

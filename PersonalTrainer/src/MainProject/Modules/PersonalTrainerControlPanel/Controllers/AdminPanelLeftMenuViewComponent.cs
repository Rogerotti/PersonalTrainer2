using Framework.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PersonalTrainerControlPanel.Controllers
{
    public class AdminPanelLeftMenuViewComponent : ViewComponent
    {
        private readonly IUserManagement userManagement;

        public AdminPanelLeftMenuViewComponent(IUserManagement userManagement)
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

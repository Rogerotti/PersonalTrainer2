using Framework.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTrainerControlPanel.Controllers
{
    public class UsersManagementTableViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<UserDto> users)
        {
            return await Task.Run(() =>
            {
                return View(users);
            });
        }
    }
}

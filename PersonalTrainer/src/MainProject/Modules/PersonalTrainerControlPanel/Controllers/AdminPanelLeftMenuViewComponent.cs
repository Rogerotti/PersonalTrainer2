using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PersonalTrainerControlPanel.Controllers
{
    public class AdminPanelLeftMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() =>
            {
                return View();
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PersonalTrainerDiet.Controllers
{
    public class LeftMenuViewComponent : ViewComponent
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

using Framework.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTrainerDiet.Controllers
{
    public class AddProductTableViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<ProductDto> products)
        {
            return await Task.Run(() =>
            {
                return View(products);
            });
        }
    }
}

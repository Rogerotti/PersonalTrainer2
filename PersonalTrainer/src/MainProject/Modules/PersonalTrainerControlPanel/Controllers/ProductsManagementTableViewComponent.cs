using Framework.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTrainerControlPanel.Controllers
{
    public class ProductsManagementTableViewComponent : ViewComponent
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

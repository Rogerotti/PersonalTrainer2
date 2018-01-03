using Framework.Models.Dto;
using Framework.Models.View;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTrainerDiet.Controllers
{
    public class DailyProductTableViewComponent : ViewComponent
    {
        public async Task <IViewComponentResult> InvokeAsync(String id, IEnumerable<DailyProductDto> list)
        {
            return await Task.Run(() =>
            {
                var dto = new DailyProductTableViewDto();
                dto.List = list;
                dto.Id = id;
                return View(dto);
            });
        }
    }
}

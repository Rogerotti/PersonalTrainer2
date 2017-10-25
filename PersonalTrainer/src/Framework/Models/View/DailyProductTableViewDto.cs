using Framework.Models.Dto;
using System;
using System.Collections.Generic;

namespace Framework.Models.View
{
    public class DailyProductTableViewDto
    {
        public String Id { get; set; }

        public IEnumerable<DailyProductDto> List { get; set; }
    }
}

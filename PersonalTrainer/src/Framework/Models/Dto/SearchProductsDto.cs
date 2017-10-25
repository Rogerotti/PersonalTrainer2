using System;
using System.Collections.Generic;

namespace Framework.Models.Dto
{
    public class SearchProductsDto
    {
        public DateTime Day { get; set; }

        public IEnumerable<ProductDto> AllProducts { get; set; }

        public IEnumerable<ProductDto> UserProducts { get; set; }

        public IEnumerable<ProductDto> RecentProducts { get; set; }
    }
}

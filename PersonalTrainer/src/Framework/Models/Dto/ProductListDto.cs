using System.Collections.Generic;

namespace Framework.Models.Dto
{
    public class ProductListDto
    {
        public IEnumerable<ProductDto> ProductList { get; set; }

        public ProductDto SelectedProduct { get; set; }
    }
}

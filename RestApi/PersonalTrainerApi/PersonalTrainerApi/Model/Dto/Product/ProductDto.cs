using PersonalTrainerApi.Model.Types;
using System;

namespace PersonalTrainerApi.Model.Dto.Product
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public String Name { get; set; }

        public String Manufacturer { get; set; }

        public ProductType Type { get; set; }

        public String TypeDisplayName { get; set; }

        public Macro Macro { get; set; }

        public ProductState State { get; set; }
    }
}

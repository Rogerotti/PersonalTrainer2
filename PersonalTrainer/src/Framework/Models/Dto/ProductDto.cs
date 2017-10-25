using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Framework.Models.Dto
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public String Name { get; set; }

        public String Manufacturer { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProductType Type { get; set; }

        public String TypeDisplayName { get; set; }
        
        public Macro Macro { get; set; }

        public ProductState State { get; set; }
    }
}

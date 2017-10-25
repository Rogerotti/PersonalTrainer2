using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Framework.Models.View
{
    public class AddEditProductView
    {
        public Guid ProductId { get; set; }

        public String Name { get; set; }

        public String Manufacturer { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProductType Type { get; set; }

        public String TypeDisplayName { get; set; }

        public Macro Macro { get; set; }

        public Mode Mode { get; set; }
    }
}

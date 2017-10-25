using Framework.Resources;
using System.ComponentModel.DataAnnotations;

namespace Framework.Models
{
    public enum ProductState
    {
        [Display(Name = nameof(EnumNames.ProductStatePrivate), ResourceType = typeof(EnumNames))]
        Private,
        [Display(Name = nameof(EnumNames.ProductStatePending), ResourceType = typeof(EnumNames))]
        Pending,
        [Display(Name = nameof(EnumNames.ProductStateDenied), ResourceType = typeof(EnumNames))]
        Denied,
        [Display(Name = nameof(EnumNames.ProductStateAccepted), ResourceType = typeof(EnumNames))]
        Accepted,
    }
}

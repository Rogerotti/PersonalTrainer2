using Framework.Resources;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Framework.Models
{
    /// <summary>
    /// Typ produktu
    /// </summary>
    public enum ProductType
    {
        [Display(Name = nameof(EnumNames.ProductTypeDairyProducts), ResourceType = typeof(EnumNames))]
        DairyProducts,
        [Display(Name = nameof(EnumNames.ProductTypeSweets), ResourceType = typeof(EnumNames))]
        Sweets,
        [Display(Name = nameof(EnumNames.ProductTypeFastFood), ResourceType = typeof(EnumNames))]
        FastFood,
        [Display(Name = nameof(EnumNames.ProductTypeFruits), ResourceType = typeof(EnumNames))]
        Fruits,
        [Display(Name = nameof(EnumNames.ProductTypeVegetables), ResourceType = typeof(EnumNames))]
        Vegetables

    }
}

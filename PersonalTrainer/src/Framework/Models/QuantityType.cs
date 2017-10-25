using Framework.Resources;
using System.ComponentModel.DataAnnotations;

namespace Framework.Models
{
    /// <summary>
    /// Typ ilości pożywienia
    /// grams - standardowo dla produktów w postaci stałej gramy.
    /// milliliters - standardowo dla produktów w postaci ciekłej mililitry.
    /// piece - jedna sztuka z opakowania.
    /// package - jedno opakowanie produktu.
    /// </summary>
    public enum QuantityType
    {
        [Display(Name = nameof(EnumNames.QuantityTypeGrams), ResourceType = typeof(EnumNames))]
        Grams,
        [Display(Name = nameof(EnumNames.QuantityTypeMilliliters), ResourceType = typeof(EnumNames))]
        Milliliters,
        [Display(Name = nameof(EnumNames.QuantityTypePiece), ResourceType = typeof(EnumNames))]
        Piece,
        [Display(Name = nameof(EnumNames.QuantityTypePackage), ResourceType = typeof(EnumNames))]
        Package
    }
}

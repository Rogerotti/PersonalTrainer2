using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PersonalTrainerApi.Extensions
{
    public static class EnumExtensions
    {
        public static String GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}

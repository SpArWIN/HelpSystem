using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HelpSystem.Domain.Extension
{
    public static class EnumEx
    {
        public static string GetDisplayName(this System.Enum enumValue)
        {
            var displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.Name;

            return displayName ?? enumValue.ToString();
        }
    }
}

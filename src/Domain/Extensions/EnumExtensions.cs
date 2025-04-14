using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString())[0];
        DisplayAttribute? displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }
}

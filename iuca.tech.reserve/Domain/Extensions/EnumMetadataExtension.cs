using Domain.Attributes;

namespace Domain.Extensions;

public static class EnumMetadataExtension
{
    public static string GetColorClass<T>(this T enumValue) where T : Enum
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = field?.GetCustomAttributes(typeof(EnumMetadataAttribute), false)
                            .FirstOrDefault() as EnumMetadataAttribute;
        return attribute?.ColorClass ?? "secondary";
    }

    public static string GetIconClass<T>(this T enumValue) where T : Enum
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = field?.GetCustomAttributes(typeof(EnumMetadataAttribute), false)
                            .FirstOrDefault() as EnumMetadataAttribute;
        return attribute?.IconClass ?? "fa-question";
    }
}

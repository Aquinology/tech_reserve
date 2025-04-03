namespace Domain.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EnumMetadataAttribute : Attribute
{
    public string ColorClass { get; }
    public string IconClass { get; }

    public EnumMetadataAttribute(string colorClass = "", string iconClass = "")
    {
        ColorClass = colorClass;
        IconClass = iconClass;
    }
}

namespace Domain.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EnumMetadataAttribute : Attribute
{
    public string TextColorClass { get; }

    public EnumMetadataAttribute(string textColorClass)
    {
        TextColorClass = textColorClass;
    }
}

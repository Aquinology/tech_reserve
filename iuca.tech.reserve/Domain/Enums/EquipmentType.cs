using Domain.Attributes;

namespace Domain.Enums;

public enum EquipmentType
{
    Unknown = 0,
    [EnumMetadata(iconClass: "fa-laptop")]
    Laptop,
    [EnumMetadata(iconClass: "fa-bolt")]
    Charger,
    [EnumMetadata(iconClass: "fa-mouse-pointer")]
    Mouse
}

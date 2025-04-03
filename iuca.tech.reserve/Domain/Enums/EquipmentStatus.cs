using Domain.Attributes;

namespace Domain.Enums;

public enum EquipmentStatus
{
    None = 0,
    [EnumMetadata(colorClass: "success")]
    Available,
    [EnumMetadata(colorClass: "warning")]
    Reserved,
    [EnumMetadata(colorClass: "danger")]
    Occupied
}

using Domain.Attributes;

namespace Domain.Enums;

public enum EquipmentStatus
{
    [EnumMetadata("secondary")]
    None = 0,
    [EnumMetadata("success")]
    Available,
    [EnumMetadata("warning")]
    Reserved,
    [EnumMetadata("danger")]
    Occupied
}

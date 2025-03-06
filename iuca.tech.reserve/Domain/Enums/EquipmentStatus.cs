using Domain.Attributes;

namespace Domain.Enums;

public enum EquipmentStatus
{
    None = 0,
    [EnumMetadata("text-success")]
    Available,
    [EnumMetadata("text-warning")]
    Reserved,
    [EnumMetadata("text-danger")]
    Occupied
}

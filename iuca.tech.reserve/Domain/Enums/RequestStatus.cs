using Domain.Attributes;

namespace Domain.Enums;

public enum RequestStatus
{
    None = 0,
    [EnumMetadata(colorClass: "secondary")]
    Canceled,
    [EnumMetadata(colorClass: "warning")]
    Pending,
    [EnumMetadata(colorClass: "danger")]
    Rejected,
    [EnumMetadata(colorClass: "primary")]
    Issued,
    [EnumMetadata(colorClass: "success")]
    Returned
}

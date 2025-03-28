using Domain.Attributes;

namespace Domain.Enums;

public enum RequestStatus
{
    [EnumMetadata("secondary")]
    None = 0,
    [EnumMetadata("secondary")]
    Canceled,
    [EnumMetadata("warning")]
    Pending,
    [EnumMetadata("danger")]
    Rejected,
    [EnumMetadata("primary")]
    Issued,
    [EnumMetadata("success")]
    Returned
}

using Domain.Attributes;

namespace Domain.Enums;

public enum RequestStatus
{
    None = 0,
    [EnumMetadata("text-secondary")]
    Canceled,
    [EnumMetadata("text-warning")]
    Pending,
    [EnumMetadata("text-danger")]
    Rejected,
    [EnumMetadata("text-primary")]
    Issued,
    [EnumMetadata("text-success")]
    Returned
}

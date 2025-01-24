using Domain.Enums;

namespace Domain.Entities;

public class Equipment
{
    public int Id { get; set; }

    public int EquipmentNumber { get; set; }
    public string? SerialNumber { get; set; }

    public string? ImgLink { get; set; }

    public EquipmentType Type { get; set; }
    public EquipmentStatus Status { get; set; }

    public IList<EquipmentRequest> EquipmentRequests { get; private set; } = new List<EquipmentRequest>();
}

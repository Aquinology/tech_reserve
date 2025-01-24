namespace Domain.Entities;

public class EquipmentRequest
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;

    public int RequestId { get; set; }
    public Request Request { get; set; } = null!;
}

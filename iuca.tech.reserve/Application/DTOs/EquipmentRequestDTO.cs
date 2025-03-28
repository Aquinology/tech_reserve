namespace Application.DTOs;

public class EquipmentRequestDTO
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }
    public EquipmentDTO? Equipment { get; set; }

    public int RequestId { get; set; }
    public RequestDTO? Request { get; set; }
}

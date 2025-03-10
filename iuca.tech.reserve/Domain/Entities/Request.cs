using Domain.Enums;

namespace Domain.Entities;

public class Request
{
    public int Id { get; set; }

    public string ClientId { get; set; } = null!;
    public Client Client { get; set; } = null!;

    public RequestStatus Status { get; set; }

    public DateTime ReservedDate { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime ReturnedDate { get; set; }

    public string? Comment { get; set; }

    public IList<EquipmentRequest> RequestEquipments { get; private set; } = new List<EquipmentRequest>();
}

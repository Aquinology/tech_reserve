using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class RequestDTO
{
    public int Id { get; set; }

    public string? ClientId { get; set; }
    public ClientDTO Client { get; set; } = new ClientDTO();

    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    public DateTime ReservedDate { get; set; } = DateTime.MinValue;
    public DateTime IssuedDate { get; set; } = DateTime.MinValue;
    public DateTime ReturnedDate { get; set; } = DateTime.MinValue;

    [MaxLength(1024)]
    public string? Comment { get; set; }

    public IList<EquipmentRequestDTO> RequestEquipments { get; set; } = new List<EquipmentRequestDTO>();
}

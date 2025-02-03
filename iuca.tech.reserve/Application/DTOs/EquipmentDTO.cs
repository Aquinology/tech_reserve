using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class EquipmentDTO
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int EquipmentNumber { get; set; }

    [MaxLength(32)]
    public string? SerialNumber { get; set; }

    [MaxLength(256)]
    public string? ImgLink { get; set; }

    public EquipmentType Type { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;

    public IList<EquipmentRequestDTO> EquipmentRequests { get; set; } = new List<EquipmentRequestDTO>();
}

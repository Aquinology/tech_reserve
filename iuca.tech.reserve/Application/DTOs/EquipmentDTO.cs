using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class EquipmentDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Equipment number is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Equipment number must be greater than 0.")]
    public int EquipmentNumber { get; set; } = 0;

    [MaxLength(32, ErrorMessage = "Serial number cannot exceed 32 characters.")]
    public string? SerialNumber { get; set; }

    [MaxLength(1024, ErrorMessage = "Serial number cannot exceed 1024 characters.")]
    public string? Description { get; set; }

    [MaxLength(256, ErrorMessage = "Image path cannot exceed 256 characters.")]
    public string? ImagePath { get; set; }

    public IFormFile? ImageFile { get; set; }

    public EquipmentType Type { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;

    public ClientDTO? Borrower {  get; set; }

    public IList<EquipmentRequestDTO> EquipmentRequests { get; set; } = new List<EquipmentRequestDTO>();
}

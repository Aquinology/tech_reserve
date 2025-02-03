using Application.DTOs;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Interfaces;

public interface IEquipmentService
{
    Task<Result<IList<EquipmentDTO>>> GetAllEquipments();
    Task<Result> CreateEquipment(EquipmentDTO equipmentDto);
    Task<Result> EditEquipment(int equipmentId, EquipmentDTO equipmentDto);
    Task<Result> DeleteEquipment(int equipmentId);
    Task<Result> SetEquipmentStatus(int equipmentId, EquipmentStatus status);
}

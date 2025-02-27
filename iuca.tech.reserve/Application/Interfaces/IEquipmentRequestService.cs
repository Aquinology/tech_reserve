using Application.DTOs.Common;

namespace Application.Interfaces;

public interface IEquipmentRequestService
{
    Task<Result> AddEquipmentToRequest(string clientId, int equipmentId);
    Task<Result> RemoveEquipmentFromRequest(string clientId, int equipmentId);
}
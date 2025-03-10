using Application.DTOs.Common;
using Application.Interfaces;
using Application.Interfaces.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class EquipmentRequestService : IEquipmentRequestService
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<EquipmentRequestService> _logger;
    private readonly IRequestService _requestService;
    private readonly IEquipmentService _equipmentService;

    public EquipmentRequestService(IApplicationDbContext db,
        IMapper mapper,
        ILogger<EquipmentRequestService> logger,
        IRequestService requestService,
        IEquipmentService equipmentService)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _requestService = requestService;
        _equipmentService = equipmentService;
    }

    public async Task<Result> AddEquipmentToRequest(string clientId, int equipmentId)
    {
        try
        {
            var requestIdResult = await _requestService.EnsurePendingRequestId(clientId);

            if (!requestIdResult.IsSuccess)
            {
                return Result.Error(requestIdResult.Message);
            }

            var equipmentResult = await _equipmentService.GetEquipment(equipmentId);

            if (!equipmentResult.IsSuccess)
            {
                return Result.Error(equipmentResult.Message);
            }

            if (equipmentResult.Data.Status == EquipmentStatus.Occupied)
            {
                return Result.Error("This equipment is currently occupied.");
            } else if (equipmentResult.Data.Status == EquipmentStatus.Reserved)
            {
                return Result.Error("This equipment is already reserved.");
            }

            var existingEquipmentInRequest = await _db.EquipmentRequests
                .AsNoTracking()
                .AnyAsync(x => x.RequestId == requestIdResult.Data && x.EquipmentId == equipmentId);

            if (existingEquipmentInRequest)
            {
                return Result.Error("You have already reserved this equipment.");
            }

            var equipmentRequest = new EquipmentRequest()
            {
                RequestId = requestIdResult.Data,
                EquipmentId = equipmentId
            };

            await _equipmentService.SetEquipmentStatus(equipmentId, EquipmentStatus.Reserved);

            await _db.EquipmentRequests.AddAsync(equipmentRequest);
            await _db.SaveChangesAsync();

            return Result.Success("Equipment added to request successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding equipment to request for client {ClientId}: {Message}", clientId, ex.Message);
            return Result.Error($"An error occurred while adding equipment to request for client {clientId}.");
        }
    }

    public async Task<Result> RemoveEquipmentFromRequest(string clientId, int equipmentId)
    {
        try
        {
            var requestIdResult = await _requestService.EnsurePendingRequestId(clientId);

            if (!requestIdResult.IsSuccess)
            {
                return Result.Error(requestIdResult.Message);
            }

            var equipmentResult = await _equipmentService.GetEquipment(equipmentId);

            if (!equipmentResult.IsSuccess)
            {
                return Result.Error(equipmentResult.Message);
            }

            if (equipmentResult.Data.Status == EquipmentStatus.Occupied)
            {
                return Result.Error("You cannot remove equipment that is already occupied.");
            }

            var equipmentRequest = _db.EquipmentRequests
                .AsNoTracking()
                .FirstOrDefault(x => x.RequestId == requestIdResult.Data &&
                    x.EquipmentId == equipmentId);

            if (equipmentRequest == null)
            {
                return Result.Error($"This equipment not found in request {requestIdResult.Data}.");
            }

            await _equipmentService.SetEquipmentStatus(equipmentId, EquipmentStatus.Available);

            _db.EquipmentRequests.Remove(equipmentRequest);
            await _db.SaveChangesAsync();

            return Result.Success("Equipment removed from request successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing equipment from request for client {ClientId}: {Message}", clientId, ex.Message);
            return Result.Error($"An error occurred while removing equipment from request for client {clientId}.");
        }
    }

    public async Task<Result> CancelExpiredRequests()
    {
        try
        {
            var expiredRequestsResult = await _requestService.GetExpiredRequests();

            if (!expiredRequestsResult.IsSuccess)
            {
                return Result.Error(expiredRequestsResult.Message);
            }

            if (expiredRequestsResult.Data.Any())
            {
                foreach (var request in expiredRequestsResult.Data)
                {
                    await _requestService.SetRequestStatus(request.Id, RequestStatus.Canceled);

                    foreach (var equipment in request.RequestEquipments)
                    {
                        await _equipmentService.SetEquipmentStatus(equipment.EquipmentId, EquipmentStatus.Available);
                    }
                }
            }

            return Result.Success("Expired requests cancelled successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling expired requests: {Message}", ex.Message);
            return Result.Error($"An error occurred while cancelling the expired requests.");
        }
    }
}

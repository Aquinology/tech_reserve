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
            if (string.IsNullOrEmpty(clientId))
            {
                return Result.Error("clientId is null.");
            }

            if (equipmentId == 0)
            {
                return Result.Error("equipmentId is 0.");
            }

            var result = await _requestService.EnsurePendingRequestId(clientId);

            if (!result.IsSuccess)
            {
                return Result.Error(result.Message);
            }

            var equipmentRequest = new EquipmentRequest()
            {
                RequestId = result.Data,
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
            if (string.IsNullOrEmpty(clientId))
            {
                return Result.Error("clientId is null.");
            }

            if (equipmentId == 0)
            {
                return Result.Error("equipmentId is 0.");
            }

            var result = await _requestService.EnsurePendingRequestId(clientId);

            if (!result.IsSuccess)
            {
                return Result.Error(result.Message);
            }

            var equipmentRequest = _db.EquipmentRequests
                .AsNoTracking()
                .FirstOrDefault(x => x.RequestId == result.Data &&
                    x.EquipmentId == equipmentId);

            if (equipmentRequest == null)
            {
                return Result.Error($"This equipment not found in request {result.Data}.");
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
}

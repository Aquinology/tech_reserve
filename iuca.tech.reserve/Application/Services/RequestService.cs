using Application.DTOs;
using Application.DTOs.Common;
using Application.Interfaces;
using Application.Interfaces.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class RequestService : IRequestService
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<RequestService> _logger;

    public RequestService(IApplicationDbContext db,
        IMapper mapper,
        ILogger<RequestService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IList<RequestDTO>>> GetRequests(RequestStatus? status = null)
    {
        try
        {
            var query = _db.Requests.AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
            } else
            {
                query = query.Where(x => x.Status == RequestStatus.Pending || x.Status == RequestStatus.Issued);
            }

            var requests = await query
                .Include(x => x.Client)
                .Include(x => x.RequestEquipments)
                .ThenInclude(x => x.Equipment)
                .OrderByDescending(x => x.Status == RequestStatus.Pending)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return Result<IList<RequestDTO>>.Success(_mapper.Map<IList<RequestDTO>>(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting requests: {Message}", ex.Message);
            return Result<IList<RequestDTO>>.Error("An error occurred while getting requests.");
        }
    }

    public async Task<Result<IList<RequestDTO>>> GetExpiredRequests()
    {
        try
        {
            var requests = await _db.Requests
                .Include(x => x.RequestEquipments)
                .Where(x => x.Status == RequestStatus.Pending &&
                    x.ReservedDate.AddMinutes(5) <= DateTime.UtcNow)
                .ToListAsync();

            return Result<IList<RequestDTO>>.Success(_mapper.Map<IList<RequestDTO>>(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired requests: {Message}", ex.Message);
            return Result<IList<RequestDTO>>.Error("An error occurred while getting expired requests.");
        }
    }

    public async Task<Result<RequestDTO>> GetActualRequest(string clientId)
    {
        try
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return Result<RequestDTO>.Error("clientId is null or empty.");
            }

            var request = await _db.Requests
                .AsNoTracking()
                .Include(x => x.Client)
                .Include(x => x.RequestEquipments)
                .ThenInclude(x => x.Equipment)
                .FirstOrDefaultAsync(x => x.ClientId == clientId &&
                    (x.Status == RequestStatus.Pending ||
                    x.Status == RequestStatus.Issued));

            return Result<RequestDTO>.Success(_mapper.Map<RequestDTO>(request));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting request for client with id {ClientId}: {Message}", clientId, ex.Message);
            return Result<RequestDTO>.Error($"An error occurred while getting request for client with id {clientId}.");
        }
    }

    public async Task<Result<int>> EnsurePendingRequestId(string clientId)
    {
        try
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return Result<int>.Error("clientId is null or empty.");
            }

            var existingRequest = await _db.Requests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ClientId == clientId &&
                    x.Status == RequestStatus.Pending);

            if (existingRequest != null)
            {
                return Result<int>.Success(existingRequest.Id, "Request found successfully.");
            }

            var issuedRequestExists = await _db.Requests
                .AsNoTracking()
                .AnyAsync(x => x.ClientId == clientId &&
                    x.Status == RequestStatus.Issued);

            if (issuedRequestExists)
            {
                return Result<int>.Error("You cannot take the equipment until you return the previous one.");
            }

            var request = new Request()
            {
                ClientId = clientId,
                Status = RequestStatus.Pending,
                ReservedDate = DateTime.UtcNow,
                IssuedDate = DateTime.MinValue,
                ReturnedDate = DateTime.MinValue
            };

            await _db.Requests.AddAsync(request);
            await _db.SaveChangesAsync();

            return Result<int>.Success(request.Id, "Request created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating request for client with id {ClientId}: {Message}", clientId, ex.Message);
            return Result<int>.Error($"An error occurred while creating request for client with id {clientId}.");
        }
    }

    public async Task<Result> SetRequestStatus(int requestId, RequestStatus status)
    {
        try
        {
            if (requestId <= 0)
            {
                return Result.Error($"requestId contains an invalid value ({requestId}).");
            }

            var request = await _db.Requests
                .Include(x => x.RequestEquipments)
                .ThenInclude(x => x.Equipment)
                .FirstOrDefaultAsync(x => x.Id == requestId);

            if (request == null)
            {
                return Result.Error($"Request with id {requestId} not found.");
            }

            request.Status = status;

            var equipmentStatus = status switch
            {
                RequestStatus.Pending => EquipmentStatus.Reserved,
                RequestStatus.Issued => EquipmentStatus.Occupied,
                _ => EquipmentStatus.Available
            };

            foreach (var equipment in request.RequestEquipments)
            {
                equipment.Equipment.Status = equipmentStatus;
            }

            await _db.SaveChangesAsync();

            return Result.Success("Request status set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting status for request with id {RequestId}: {Message}", requestId, ex.Message);
            return Result.Error($"An error occurred while setting status for request with id {requestId}.");
        }
    }

    public async Task<Result> SetReservedDate(int requestId, DateTime date)
    {
        try
        {
            if (requestId <= 0)
            {
                return Result.Error($"requestId contains an invalid value ({requestId}).");
            }

            var request = await _db.Requests.FindAsync(requestId);

            if (request == null)
            {
                return Result.Error($"Request with id {requestId} not found.");
            }

            request.ReservedDate = date;
            await _db.SaveChangesAsync();

            return Result.Success("Request reserved date set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting reserved date for request with id {RequestId}: {Message}", requestId, ex.Message);
            return Result.Error($"An error occurred while setting reserved date for request with id {requestId}.");
        }
    }

    public async Task<Result> SetIssuedDate(int requestId, DateTime date)
    {
        try
        {
            if (requestId <= 0)
            {
                return Result.Error($"requestId contains an invalid value ({requestId}).");
            }

            var request = await _db.Requests.FindAsync(requestId);

            if (request == null)
            {
                return Result.Error($"Request with id {requestId} not found.");
            }

            request.IssuedDate = date;
            await _db.SaveChangesAsync();

            return Result.Success("Request issued date set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting issued date for request with id {RequestId}: {Message}", requestId, ex.Message);
            return Result.Error($"An error occurred while setting issued date for request with id {requestId}.");
        }
    }

    public async Task<Result> SetReturnedDate(int requestId, DateTime date)
    {
        try
        {
            if (requestId <= 0)
            {
                return Result.Error($"requestId contains an invalid value ({requestId}).");
            }

            var request = await _db.Requests.FindAsync(requestId);

            if (request == null)
            {
                return Result.Error($"Request with id {requestId} not found.");
            }

            request.ReturnedDate = date;
            await _db.SaveChangesAsync();

            return Result.Success("Request returned date set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting returned date for request with id {RequestId}: {Message}", requestId, ex.Message);
            return Result.Error($"An error occurred while setting returned date for request with id {requestId}.");
        }
    }
}

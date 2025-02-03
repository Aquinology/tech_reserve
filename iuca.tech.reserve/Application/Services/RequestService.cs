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
    private readonly ILogger<UserService> _logger;

    public RequestService(IApplicationDbContext db,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IList<RequestDTO>>> GetAllRequests()
    {
        try
        {
            var requests = await _db.Requests
                .AsNoTracking()
                .ToListAsync();

            return Result<IList<RequestDTO>>.Success(_mapper.Map<IList<RequestDTO>>(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting requests: {Message}", ex.Message);
            return Result<IList<RequestDTO>>.Error("An error occurred while getting requests.");
        }
    }

    public async Task<Result> CreateRequest(RequestDTO requestDto)
    {
        try
        {
            if (requestDto == null)
            {
                return Result.Error("requestDto is null.");
            }

            var requestExists = await _db.Requests
                .AnyAsync(x => x.ClientId == requestDto.ClientId);

            if (requestExists)
            {
                return Result.Error($"Request for client with id {requestDto.ClientId} already exists.");
            }

            var request = _mapper.Map<Request>(requestDto);
            await _db.Requests.AddAsync(request);
            await _db.SaveChangesAsync();

            return Result.Success("Request created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating request: {Message}", ex.Message);
            return Result.Error("An error occurred while creating request.");
        }
    }

    public async Task<Result> SetRequestStatus(int requestId, RequestStatus status)
    {
        try
        {
            if (requestId == 0)
            {
                return Result.Error("requestId is 0.");
            }

            var requests = await _db.Requests.FindAsync(requestId);

            if (requests == null)
            {
                return Result.Error($"Request with id {requestId} not found.");
            }

            requests.Status = status;
            await _db.SaveChangesAsync();

            return Result.Success("Request status set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting request status: {Message}", ex.Message);
            return Result.Error("An error occurred while setting request status.");
        }
    }

    public async Task<Result> SetIssuedDate(int requestId, DateTime date)
    {
        try
        {
            if (requestId == 0)
            {
                return Result.Error("requestId is 0.");
            }

            var requests = await _db.Requests.FindAsync(requestId);

            if (requests == null)
            {
                return Result.Error($"Request with id {requestId} not found.");
            }

            requests.IssuedDate = date;
            await _db.SaveChangesAsync();

            return Result.Success("Request issued date set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting request issued date: {Message}", ex.Message);
            return Result.Error("An error occurred while setting request issued date.");
        }
    }

    public async Task<Result> SetReturnedDate(int requestId, DateTime date)
    {
        try
        {
            if (requestId == 0)
            {
                return Result.Error("requestId is 0.");
            }

            var requests = await _db.Requests.FindAsync(requestId);

            if (requests == null)
            {
                return Result.Error($"Request with id {requestId} not found.");
            }

            requests.ReturnedDate = date;
            await _db.SaveChangesAsync();

            return Result.Success("Request returned date set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting request returned date: {Message}", ex.Message);
            return Result.Error("An error occurred while setting request returned date.");
        }
    }
}

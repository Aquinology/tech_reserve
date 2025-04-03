using Application.DTOs;
using Application.DTOs.Common;
using Application.Interfaces;
using Application.Interfaces.Common;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ClientService : IClientService
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<ClientService> _logger;

    public ClientService(IApplicationDbContext db,
        IMapper mapper,
        ILogger<ClientService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IList<ClientDTO>>> GetClients()
    {
        try
        {
            var clients = await _db.Clients
                .AsNoTracking()
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();

            return Result<IList<ClientDTO>>.Success(_mapper.Map<IList<ClientDTO>>(clients));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients: {Message}", ex.Message);
            return Result<IList<ClientDTO>>.Error("An error occurred while getting clients.");
        }
    }

    public async Task<Result> CreateClient(ClientDTO clientDto)
    {
        try
        {
            if (clientDto == null)
            {
                return Result.Error("clientDto is null.");
            }

            var clientExists = await _db.Clients
                .AsNoTracking()
                .AnyAsync(x => x.ApplicationUserId == clientDto.ApplicationUserId);

            if (clientExists)
            {
                return Result.Error($"Client with id {clientDto.ApplicationUserId} already exists.");
            }

            var client = _mapper.Map<Client>(clientDto);
            await _db.Clients.AddAsync(client);
            await _db.SaveChangesAsync();

            return Result.Success("Client created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client: {Message}", ex.Message);
            return Result.Error("An error occurred while creating client.");
        }
    }

    public async Task<Result> DeleteClient(string clientId)
    {
        try
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return Result.Error($"clientId null or empty.");
            }

            var client = await _db.Clients.FindAsync(clientId);

            if (client == null)
            {
                return Result.Error($"Client with id {clientId} not found.");
            }

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();

            return Result.Success("Client deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client with id {ClientId}: {Message}", clientId, ex.Message);
            return Result.Error($"An error occurred while deleting client with id {clientId}.");
        }
    }

    public async Task<Result> UpdateClientPhoneNumber(string clientId, string phone)
    {
        try
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return Result.Error($"clientId null or empty.");
            }

            var client = await _db.Clients.FindAsync(clientId);

            if (client == null)
            {
                return Result.Error($"Client with id {clientId} not found.");
            }

            client.PhoneNumber = phone;

            await _db.SaveChangesAsync();

            return Result.Success("Client phone number updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating phone number for client with id {ClientId}: {Message}", clientId, ex.Message);
            return Result.Error($"An error occurred while updating phone number for client with id {clientId}.");
        }
    }
}

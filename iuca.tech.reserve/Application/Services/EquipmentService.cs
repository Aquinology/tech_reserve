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

public class EquipmentService : IEquipmentService
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public EquipmentService(IApplicationDbContext db,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IList<EquipmentDTO>>> GetAllEquipments()
    {
        try
        {
            var equipments = await _db.Equipments
                .AsNoTracking()
                .ToListAsync();

            return Result<IList<EquipmentDTO>>.Success(_mapper.Map<IList<EquipmentDTO>>(equipments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipments: {Message}", ex.Message);
            return Result<IList<EquipmentDTO>>.Error("An error occurred while getting equipments.");
        }
    }

    public async Task<Result> CreateEquipment(EquipmentDTO equipmentDto)
    {
        try
        {
            if (equipmentDto == null)
            {
                return Result.Error("equipmentDto is null.");
            }

            var equipmentExists = await _db.Equipments
                .AnyAsync(x => x.EquipmentNumber == equipmentDto.EquipmentNumber);

            if (equipmentExists)
            {
                return Result.Error($"Equipment with number {equipmentDto.EquipmentNumber} already exists.");
            }

            var equipment = _mapper.Map<Equipment>(equipmentDto);
            await _db.Equipments.AddAsync(equipment);
            await _db.SaveChangesAsync();

            return Result.Success("Equipment created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating equipment: {Message}", ex.Message);
            return Result.Error("An error occurred while creating equipment.");
        }
    }

    public async Task<Result> EditEquipment(int equipmentId, EquipmentDTO equipmentDto)
    {
        try
        {
            if (equipmentId == 0)
            {
                return Result.Error("equipmentId is 0.");
            }

            if (equipmentDto == null)
            {
                return Result.Error("equipmentDto is null.");
            }

            var equipment = await _db.Equipments.FindAsync(equipmentId);

            if (equipment == null)
            {
                return Result.Error($"Equipment with id {equipmentId} not found.");
            }

            if (equipment.EquipmentNumber != equipmentDto.EquipmentNumber)
            {
                var equipmentExists = await _db.Equipments
                    .AnyAsync(x => x.EquipmentNumber == equipmentDto.EquipmentNumber);

                if (equipmentExists)
                {
                    return Result.Error($"Equipment with number {equipmentDto.EquipmentNumber} already exists.");
                }
            }

            equipment.EquipmentNumber = equipmentDto.EquipmentNumber;
            equipment.SerialNumber = equipmentDto.SerialNumber;
            equipment.ImgLink = equipmentDto.ImgLink;
            equipment.Type = equipmentDto.Type;
            equipment.Status = equipmentDto.Status;

            await _db.SaveChangesAsync();

            return Result.Success("Equipment edited successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing equipment: {Message}", ex.Message);
            return Result.Error("An error occurred while editing equipment.");
        }
    }

    public async Task<Result> DeleteEquipment(int equipmentId)
    {
        try
        {
            if (equipmentId == 0)
            {
                return Result.Error("equipmentId is 0.");
            }

            var equipment = await _db.Equipments.FindAsync(equipmentId);

            if (equipment == null)
            {
                return Result.Error($"Equipment with id {equipmentId} not found.");
            }

            _db.Equipments.Remove(equipment);
            await _db.SaveChangesAsync();

            return Result.Success("Equipment deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting equipment: {Message}", ex.Message);
            return Result.Error("An error occurred while deleting equipment.");
        }
    }

    public async Task<Result> SetEquipmentStatus(int equipmentId, EquipmentStatus status)
    {
        try
        {
            if (equipmentId == 0)
            {
                return Result.Error("equipmentId is 0.");
            }

            var equipment = await _db.Equipments.FindAsync(equipmentId);

            if (equipment == null)
            {
                return Result.Error($"Equipment with id {equipmentId} not found.");
            }

            equipment.Status = status;
            await _db.SaveChangesAsync();

            return Result.Success("Equipment status set successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting equipment status: {Message}", ex.Message);
            return Result.Error("An error occurred while setting equipment status.");
        }
    }
}

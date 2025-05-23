﻿using Application.DTOs;
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
    private readonly ILogger<EquipmentService> _logger;
    private readonly IFileService _fileService;

    public EquipmentService(IApplicationDbContext db,
        IMapper mapper,
        ILogger<EquipmentService> logger,
        IFileService fileService)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task<Result<IList<EquipmentDTO>>> GetEquipments()
    {
        try
        {
            var equipments = await _db.Equipments
                .AsNoTracking()
                .Include(x => x.EquipmentRequests)
                .ThenInclude(x => x.Request.Client)
                .OrderByDescending(x => x.Status == EquipmentStatus.Available)
                .ThenBy(x => x.EquipmentNumber)
                .ToListAsync();

            var mappedEquipments = _mapper.Map<IList<EquipmentDTO>>(equipments);

            mappedEquipments.ToList().ForEach(x =>
            {
                var activeRequest = x.EquipmentRequests
                    .FirstOrDefault(y => y.Request!.Status == RequestStatus.Pending || y.Request.Status == RequestStatus.Issued);
                if (activeRequest != null)
                {
                    x.Borrower = activeRequest.Request!.Client;
                }
            });

            return Result<IList<EquipmentDTO>>.Success(mappedEquipments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipments: {Message}", ex.Message);
            return Result<IList<EquipmentDTO>>.Error("An error occurred while getting equipments.");
        }
    }

    public async Task<Result<EquipmentDTO>> GetEquipment(int equipmentId)
    {
        try
        {
            if (equipmentId <= 0)
            {
                return Result<EquipmentDTO>.Error($"equipmentId contains an invalid value ({equipmentId}).");
            }

            var equipment = await _db.Equipments.FindAsync(equipmentId);

            if (equipment == null)
            {
                return Result<EquipmentDTO>.Error($"Equipment with id {equipmentId} not found.");
            }

            return Result<EquipmentDTO>.Success(_mapper.Map<EquipmentDTO>(equipment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment with id {EquipmentId}: {Message}", equipmentId, ex.Message);
            return Result<EquipmentDTO>.Error($"An error occurred while getting equipment with id {equipmentId}.");
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
                .AsNoTracking()
                .AnyAsync(x => x.EquipmentNumber == equipmentDto.EquipmentNumber);

            if (equipmentExists)
            {
                return Result.Error($"Equipment with number {equipmentDto.EquipmentNumber} already exists.");
            }

            if (equipmentDto.ImageFile != null && equipmentDto.ImageFile.Length != 0)
            {
                equipmentDto.ImagePath = await _fileService.SaveFile(equipmentDto.ImageFile);
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
            if (equipmentId <= 0)
            {
                return Result.Error($"equipmentId contains an invalid value ({equipmentId}).");
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
                    .AsNoTracking()
                    .AnyAsync(x => x.EquipmentNumber == equipmentDto.EquipmentNumber);

                if (equipmentExists)
                {
                    return Result.Error($"Equipment with number {equipmentDto.EquipmentNumber} already exists.");
                }
            }

            if (equipmentDto.ImageFile != null && equipmentDto.ImageFile.Length != 0)
            {
                if (!string.IsNullOrEmpty(equipment.ImagePath))
                {
                    _fileService.DeleteFile(equipment.ImagePath);
                }

                equipment.ImagePath = await _fileService.SaveFile(equipmentDto.ImageFile);
            }

            equipment.EquipmentNumber = equipmentDto.EquipmentNumber;
            equipment.SerialNumber = equipmentDto.SerialNumber;
            equipment.Description = equipmentDto.Description;
            equipment.Type = equipmentDto.Type;
            equipment.Status = equipmentDto.Status;

            await _db.SaveChangesAsync();

            return Result.Success("Equipment edited successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing equipment with id {EquipmentId}: {Message}", equipmentId, ex.Message);
            return Result.Error($"An error occurred while editing equipment with id {equipmentId}.");
        }
    }

    public async Task<Result> DeleteEquipment(int equipmentId)
    {
        try
        {
            if (equipmentId <= 0)
            {
                return Result.Error($"equipmentId contains an invalid value ({equipmentId}).");
            }

            var equipment = await _db.Equipments.FindAsync(equipmentId);

            if (equipment == null)
            {
                return Result.Error($"Equipment with id {equipmentId} not found.");
            }

            if (!string.IsNullOrEmpty(equipment.ImagePath))
            {
                _fileService.DeleteFile(equipment.ImagePath);
            }

            _db.Equipments.Remove(equipment);
            await _db.SaveChangesAsync();

            return Result.Success("Equipment deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting equipment with id {EquipmentId}: {Message}", equipmentId, ex.Message);
            return Result.Error($"An error occurred while deleting equipment with id {equipmentId}.");
        }
    }

    public async Task<Result> SetEquipmentStatus(int equipmentId, EquipmentStatus status)
    {
        try
        {
            if (equipmentId <= 0)
            {
                return Result.Error($"equipmentId contains an invalid value ({equipmentId}).");
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
            _logger.LogError(ex, "Error setting status for equipment with id {EquipmentId}: {Message}", equipmentId, ex.Message);
            return Result.Error($"An error occurred while setting status for equipment with id {equipmentId}.");
        }
    }
}

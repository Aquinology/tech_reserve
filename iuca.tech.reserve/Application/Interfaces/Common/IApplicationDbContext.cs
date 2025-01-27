using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Common;

public interface IApplicationDbContext
{
    DbSet<Client> Clients { get; }
    DbSet<Request> Requests { get; }
    DbSet<Equipment> Equipments { get; }
    DbSet<EquipmentRequest> EquipmentRequests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

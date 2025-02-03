using Application.DTOs.Common;
using Application.Interfaces;
using Application.Interfaces.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Npgsql;

namespace Application.Services;

public class ImportDataService : IImportDataService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<ImportDataService> _logger;

    public ImportDataService(IApplicationDbContext db,
        ILogger<ImportDataService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> ImportClients(string connection)
    {
        try
        {
            var existingClients = await _db.Clients
                .AsNoTracking()
                .ToDictionaryAsync(u => u.ApplicationUserId!);

            await using var conn = new NpgsqlConnection(connection);
            await conn.OpenAsync();

            string query = @"
                SELECT DISTINCT u.*, 
                    soi.""StudentId"", 
                    soi.""State"",
                    d.""Code"" as ""Department"",
                    dg.""Code"" as ""Group"",
                    sci.""Phone""
                FROM public.""AspNetUsers"" u
                JOIN public.""AspNetUserRoles"" ur ON u.""Id"" = ur.""UserId""
                JOIN public.""AspNetRoles"" r ON ur.""RoleId"" = r.""Id""
                JOIN public.""StudentBasicInfo"" sbi ON u.""Id"" = sbi.""ApplicationUserId""
                JOIN public.""StudentOrgInfo"" soi ON sbi.""Id"" = soi.""StudentBasicInfoId""
                LEFT JOIN public.""StudentContactInfo"" sci ON sbi.""Id"" = sci.""StudentBasicInfoId""
                LEFT JOIN public.""DepartmentGroups"" dg ON soi.""DepartmentGroupId"" = dg.""Id""
                LEFT JOIN public.""Departments"" d ON dg.""DepartmentId"" = d.""Id""
                WHERE r.""Name"" IN('Student_1', 'Student_2') AND soi.""State"" = 1";

            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            var clientsToAdd = new List<Client>();
            var clientsToUpdate = new List<Client>();

            while (await reader.ReadAsync())
            {
                string clientId = reader.GetString(reader.GetOrdinal("Id"));
                string email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email"));
                string firstName = reader.IsDBNull(reader.GetOrdinal("FirstNameEng")) ? "" : reader.GetString(reader.GetOrdinal("FirstNameEng"));
                string lastName = reader.IsDBNull(reader.GetOrdinal("LastNameEng")) ? "" : reader.GetString(reader.GetOrdinal("LastNameEng"));
                string phoneNumber = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString(reader.GetOrdinal("Phone"));

                string groupCode = reader.IsDBNull(reader.GetOrdinal("Group")) ? "NA" : reader.GetString(reader.GetOrdinal("Group"));
                int group = groupCode == "NA" ? 0 : int.Parse(groupCode);
                string department = reader.IsDBNull(reader.GetOrdinal("Department")) ? "" : reader.GetString(reader.GetOrdinal("Department"));

                if (existingClients.TryGetValue(clientId, out var client))
                {
                    client.Email = email;
                    client.FirstName = firstName;
                    client.LastName = lastName;
                    client.OtherInfo = $"Group {department}-{groupCode}";
                    clientsToUpdate.Add(client);
                }
                else
                {
                    client = new Client
                    {
                        ApplicationUserId = clientId,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        PhoneNumber = phoneNumber,
                        OtherInfo = $"Group {department}-{groupCode}"
                    };
                    clientsToAdd.Add(client);
                }
            }

            _db.Clients.AddRange(clientsToAdd);
            _db.Clients.UpdateRange(clientsToUpdate);

            await _db.SaveChangesAsync();

            string message = $"Imported {clientsToAdd.Count} new clients, updated {clientsToUpdate.Count} existing clients.";
            _logger.LogInformation(message);
            return Result.Success(message);
        }
        catch (Exception ex)
        {
            string errorMessage = "Error occurred while importing clients.";
            _logger.LogError(ex, errorMessage);
            return Result.Error(errorMessage);
        }
    }
}

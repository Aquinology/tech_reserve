using Microsoft.AspNetCore.Identity;
using Application.DTOs;
using Application.DTOs.Common;

namespace Application.Interfaces;

public interface IUserService
{
    Task<Result<IList<ClientDTO>>> GetAllClients();
    Task<Result<IList<(IdentityUser User, IList<string> Roles)>>> GetAllUsersWithRoles();
    Task<Result<IList<IdentityRole>>> GetAllRoles();
    Task<Result> CreateUser(string email, string role);
    Task<Result> DeleteUser(string userId);
    Task<Result> SyncClientsWithIdentity();
}

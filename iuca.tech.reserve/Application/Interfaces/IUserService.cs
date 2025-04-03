using Application.DTOs.Common;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces;

public interface IUserService
{
    Task<Result<IList<(IdentityUser User, IList<string> Roles)>>> GetAllUsersWithRoles();
    Task<Result<IList<IdentityRole>>> GetAllRoles();
    Task<Result> CreateUser(string email, string role, string firstName = null, string lastName = null);
    Task<Result> DeleteUser(string userId);
    Task<Result> GenerateClientAccounts();
}

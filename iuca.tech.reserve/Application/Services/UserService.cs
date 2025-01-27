using Application.DTOs.Common;
using Application.DTOs;
using Application.Interfaces.Common;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IApplicationDbContext db,
            IMapper mapper,
            ILogger<UserService> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<IList<ClientDTO>>> GetAllClients()
        {
            var clients = await _db.Clients
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();

            if (clients == null || !clients.Any())
            {
                return Result<IList<ClientDTO>>.Error("No clients available at the moment.");
            }

            return Result<IList<ClientDTO>>.Success(_mapper.Map<IList<ClientDTO>>(clients));
        }

        public async Task<Result<IList<(IdentityUser User, IList<string> Roles)>>> GetAllUsersWithRoles()
        {
            var users = await _userManager.Users
                .OrderBy(x => x.Email)
                .ToListAsync();

            if (users == null || !users.Any())
            {
                return Result<IList<(IdentityUser User, IList<string> Roles)>>.Error("No users available at the moment.");
            }

            var usersWithRoles = new List<(IdentityUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add((user, roles.ToList()));
            }

            return Result<IList<(IdentityUser User, IList<string> Roles)>>.Success(usersWithRoles);
        }

        public async Task<Result<IList<IdentityRole>>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (roles == null || !roles.Any())
            {
                return Result<IList<IdentityRole>>.Error("No roles available at the moment.");
            }

            return Result<IList<IdentityRole>>.Success(roles.ToList());
        }

        public async Task<Result> CreateUser(string email, string role)
        {
            if (!email.EndsWith("@iuca.kg"))
            {
                return Result.Error("Email must end with @iuca.kg.");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Result.Error("User with this email already exists.");
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return Result.Success($"User {email} created successfully.");
            }
            else
            {
                _logger.LogError("Failed to create user {Email}. Errors: {Errors}",
                    email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return Result.Error("Failed to create user.");
            }
        }

        public async Task<Result> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Error("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Result.Success($"User {user.UserName} deleted successfully.");
            }
            else
            {
                _logger.LogError("Failed to delete user {UserId}. Errors: {Errors}",
                    userId,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return Result.Error("Failed to delete user.");
            }
        }

        public async Task<Result> SyncClientsWithIdentity()
        {
            var clients = await _db.Clients
                .Where(s => !string.IsNullOrEmpty(s.ApplicationUserId))
                .ToListAsync();

            int createdCount = 0;
            int updatedCount = 0;
            int failedCount = 0;

            foreach (var client in clients)
            {
                try
                {
                    var existingUser = await _userManager.FindByIdAsync(client.ApplicationUserId!);
                    if (existingUser != null)
                    {
                        existingUser.UserName = client.Email;
                        existingUser.Email = client.Email;
                        var updateResult = await _userManager.UpdateAsync(existingUser);

                        if (updateResult.Succeeded)
                        {
                            updatedCount++;
                        }
                        else
                        {
                            failedCount++;
                            _logger.LogError("Failed to update user for client {ClientId}. Errors: {Errors}",
                                client.ApplicationUserId,
                                string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                        }
                        continue;
                    }

                    var user = new IdentityUser
                    {
                        Id = client.ApplicationUserId!,
                        UserName = client.Email,
                        Email = client.Email
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, Roles.Client);
                        createdCount++;
                    }
                    else
                    {
                        failedCount++;
                        _logger.LogError("Failed to create user for client {ClientId}. Errors: {Errors}",
                            client.ApplicationUserId,
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _logger.LogError(ex, "Error creating or updating user for client {ClientId}", client.ApplicationUserId);
                }
            }

            await _db.SaveChangesAsync();

            var message = $"Processed {clients.Count} clients. Created: {createdCount}, Updated: {updatedCount}, Failed: {failedCount}";
            return failedCount == 0
                ? Result.Success(message)
                : Result.Error(message);
        }
    }
}

using Application.DTOs.Common;
using Application.DTOs;

namespace Application.Interfaces;

public interface IClientService
{
    Task<Result<IList<ClientDTO>>> GetClients();
    Task<Result> CreateClient(ClientDTO clientDto);
    Task<Result> DeleteClient(string clientId);
    Task<Result> UpdateClientPhoneNumber(string clientId, string phone);
}

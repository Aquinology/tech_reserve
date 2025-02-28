using Application.DTOs;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Interfaces;

public interface IRequestService
{
    Task<Result<IList<RequestDTO>>> GetRequests(RequestStatus? status = null);
    Task<Result<RequestDTO>> GetActualRequest(string clientId);
    Task<Result<int>> EnsurePendingRequestId(string clientId);
    Task<Result> SetRequestStatus(int requestId, RequestStatus status);
    Task<Result> SetIssuedDate(int requestId, DateTime date);
    Task<Result> SetReturnedDate(int requestId, DateTime date);
}

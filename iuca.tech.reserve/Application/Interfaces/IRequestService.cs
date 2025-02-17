﻿using Application.DTOs;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Interfaces;

public interface IRequestService
{
    Task<Result<IList<RequestDTO>>> GetAllRequests();
    Task<Result> CreateRequest(RequestDTO requestDto);
    Task<Result> SetRequestStatus(int requestId, RequestStatus status);
    Task<Result> SetIssuedDate(int requestId, DateTime date);
    Task<Result> SetReturnedDate(int requestId, DateTime date);
}

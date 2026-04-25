using Crm.Application.DTOs.Auth;
using FluentResults;

namespace Crm.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponse>> Login(LoginRequest request);
    Task<Result<LoginResponse>> Register(LoginRequest request);
}
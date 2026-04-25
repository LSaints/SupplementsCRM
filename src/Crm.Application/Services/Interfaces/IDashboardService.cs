using Crm.Application.DTOs.Dashboard;
using FluentResults;

namespace Crm.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardDto>> GetDados(Guid? vendedorId = null);
}
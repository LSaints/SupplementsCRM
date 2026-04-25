using Crm.Application.DTOs.Cliente;
using FluentResults;

namespace Crm.Application.Services.Interfaces;

public interface IClienteService
{
    Task<Result<List<ClienteDto>>> GetAll();
    Task<Result<List<ClienteDto>>> GetByVendedorId(Guid vendedorId);
    Task<Result<ClienteDto>> GetById(Guid id);
    Task<Result<ClienteDto>> Create(CreateClienteRequest request);
    Task<Result<ClienteDto>> Update(Guid id, UpdateClienteRequest request);
    Task<Result> Delete(Guid id);
}
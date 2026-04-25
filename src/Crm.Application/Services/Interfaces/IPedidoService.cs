using Crm.Application.DTOs.Pedido;
using Crm.Domain.Enums;
using FluentResults;

namespace Crm.Application.Services.Interfaces;

public interface IPedidoService
{
    Task<Result<List<PedidoDto>>> GetAll();
    Task<Result<List<PedidoDto>>> GetByVendedorId(Guid vendedorId);
    Task<Result<List<PedidoDto>>> GetByClienteId(Guid clienteId);
    Task<Result<PedidoDto>> GetById(Guid id);
    Task<Result<PedidoDto>> Create(CreatePedidoRequest request);
    Task<Result<PedidoDto>> Update(Guid id, CreatePedidoRequest request);
    Task<Result<PedidoDto>> UpdateStatus(Guid id, StatusPedido status);
    Task<Result> Delete(Guid id);
}
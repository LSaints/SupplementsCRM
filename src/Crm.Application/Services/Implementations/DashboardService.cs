using AutoMapper;
using Crm.Application.DTOs.Dashboard;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentResults;

namespace Crm.Application.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMapper _mapper;

    public DashboardService(
        IClienteRepository clienteRepository,
        IPedidoRepository pedidoRepository,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _pedidoRepository = pedidoRepository;
        _mapper = mapper;
    }

    public async Task<Result<DashboardDto>> GetDados(Guid? vendedorId = null)
    {
        var clientes = vendedorId.HasValue
            ? await _clienteRepository.GetByVendedorIdAsync(vendedorId.Value)
            : await _clienteRepository.GetAllAsync();

        var pedidos = vendedorId.HasValue
            ? await _pedidoRepository.GetByVendedorIdAsync(vendedorId.Value)
            : await _pedidoRepository.GetAllAsync();

        var pedidosPorStatus = pedidos
            .GroupBy(p => p.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var dashboard = new DashboardDto
        {
            TotalClientes = clientes.Count,
            TotalPedidos = pedidos.Count,
            PedidosPagos = pedidos.Count(p => p.Status == StatusPedido.Pago),
            FaturamentoTotal = pedidos
                .Where(p => p.Status == StatusPedido.Pago)
                .Sum(p => p.ValorTotal),
            PedidosStatus = pedidosPorStatus
        };

        return Result.Ok(dashboard);
    }
}
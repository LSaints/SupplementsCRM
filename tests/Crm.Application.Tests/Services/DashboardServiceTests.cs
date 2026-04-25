using AutoMapper;
using Crm.Application.DTOs.Dashboard;
using Crm.Application.Services.Implementations;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentAssertions;
using FluentResults;
using Moq;
using Xunit;

namespace Crm.Application.Tests.Services;

public class DashboardServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DashboardService _sut;

    public DashboardServiceTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new DashboardService(
            _clienteRepositoryMock.Object,
            _pedidoRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetDados_DeveRetornarDashboardComDadosGerais_QuandoNaoInformadoVendedor()
    {
        var clientes = new List<Cliente>
        {
            new() { Id = Guid.NewGuid(), Nome = "Cliente 1" },
            new() { Id = Guid.NewGuid(), Nome = "Cliente 2" }
        };
        var pedidos = new List<Pedido>
        {
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pago, ValorTotal = 100m },
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pendente, ValorTotal = 50m }
        };

        _clienteRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(clientes);
        _pedidoRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);

        var result = await _sut.GetDados();

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalClientes.Should().Be(2);
        result.Value.TotalPedidos.Should().Be(2);
        result.Value.PedidosPagos.Should().Be(1);
        result.Value.FaturamentoTotal.Should().Be(100m);
    }

    [Fact]
    public async Task GetDados_DeveRetornarDashboardDoVendedor_QuandoInformadoVendedorId()
    {
        var vendedorId = Guid.NewGuid();
        var clientes = new List<Cliente>
        {
            new() { Id = Guid.NewGuid(), Nome = "Cliente 1", VendedorId = vendedorId }
        };
        var pedidos = new List<Pedido>
        {
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pago, ValorTotal = 200m, VendedorId = vendedorId }
        };

        _clienteRepositoryMock
            .Setup(x => x.GetByVendedorIdAsync(vendedorId))
            .ReturnsAsync(clientes);
        _pedidoRepositoryMock
            .Setup(x => x.GetByVendedorIdAsync(vendedorId))
            .ReturnsAsync(pedidos);

        var result = await _sut.GetDados(vendedorId);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalClientes.Should().Be(1);
        result.Value.TotalPedidos.Should().Be(1);
        result.Value.FaturamentoTotal.Should().Be(200m);
    }

    [Fact]
    public async Task GetDados_DeveCalcularFaturamentoTotal_SomentePedidosPagos()
    {
        var pedidos = new List<Pedido>
        {
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pago, ValorTotal = 100m },
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pendente, ValorTotal = 50m },
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Cancelado, ValorTotal = 30m }
        };

        _clienteRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Cliente>());
        _pedidoRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);

        var result = await _sut.GetDados();

        result.IsSuccess.Should().BeTrue();
        result.Value.FaturamentoTotal.Should().Be(100m);
        result.Value.PedidosPagos.Should().Be(1);
    }

    [Fact]
    public async Task GetDados_DeveAgruparPedidosPorStatus()
    {
        var pedidos = new List<Pedido>
        {
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pago },
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pago },
            new() { Id = Guid.NewGuid(), Status = StatusPedido.Pendente }
        };

        _clienteRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Cliente>());
        _pedidoRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);

        var result = await _sut.GetDados();

        result.IsSuccess.Should().BeTrue();
        result.Value.PedidosStatus.Should().ContainKey("Pago");
        result.Value.PedidosStatus.Should().ContainKey("Pendente");
        result.Value.PedidosStatus["Pago"].Should().Be(2);
        result.Value.PedidosStatus["Pendente"].Should().Be(1);
    }

    [Fact]
    public async Task GetDados_DeveRetornarZeros_QuandoNaoExistiremDados()
    {
        _clienteRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Cliente>());
        _pedidoRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Pedido>());

        var result = await _sut.GetDados();

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalClientes.Should().Be(0);
        result.Value.TotalPedidos.Should().Be(0);
        result.Value.PedidosPagos.Should().Be(0);
        result.Value.FaturamentoTotal.Should().Be(0m);
    }
}
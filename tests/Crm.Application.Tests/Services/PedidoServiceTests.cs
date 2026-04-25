using AutoMapper;
using Crm.Application.DTOs.Pedido;
using Crm.Application.Services.Implementations;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentAssertions;
using FluentResults;
using Moq;
using Xunit;

namespace Crm.Application.Tests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PedidoService _sut;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _sut = new PedidoService(
            _pedidoRepositoryMock.Object,
            _produtoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaPedidos()
    {
        var pedidos = new List<Pedido>
        {
            new() { Id = Guid.NewGuid(), ValorTotal = 100m },
            new() { Id = Guid.NewGuid(), ValorTotal = 200m }
        };
        var pedidoDtos = new List<PedidoDto>
        {
            new() { Id = pedidos[0].Id, ValorTotal = 100m },
            new() { Id = pedidos[1].Id, ValorTotal = 200m }
        };

        _pedidoRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);
        _mapperMock
            .Setup(x => x.Map<List<PedidoDto>>(pedidos))
            .Returns(pedidoDtos);

        var result = await _sut.GetAll();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByVendedorId_DeveRetornarPedidosDoVendedor()
    {
        var vendedorId = Guid.NewGuid();
        var pedidos = new List<Pedido>
        {
            new() { Id = Guid.NewGuid(), VendedorId = vendedorId, ValorTotal = 100m }
        };
        var pedidoDtos = new List<PedidoDto>
        {
            new() { Id = pedidos[0].Id, VendedorId = vendedorId, ValorTotal = 100m }
        };

        _pedidoRepositoryMock
            .Setup(x => x.GetByVendedorIdAsync(vendedorId))
            .ReturnsAsync(pedidos);
        _mapperMock
            .Setup(x => x.Map<List<PedidoDto>>(pedidos))
            .Returns(pedidoDtos);

        var result = await _sut.GetByVendedorId(vendedorId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByClienteId_DeveRetornarPedidosDoCliente()
    {
        var clienteId = Guid.NewGuid();
        var pedidos = new List<Pedido>
        {
            new() { Id = Guid.NewGuid(), ClienteId = clienteId, ValorTotal = 100m }
        };
        var pedidoDtos = new List<PedidoDto>
        {
            new() { Id = pedidos[0].Id, ClienteId = clienteId, ValorTotal = 100m }
        };

        _pedidoRepositoryMock
            .Setup(x => x.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(pedidos);
        _mapperMock
            .Setup(x => x.Map<List<PedidoDto>>(pedidos))
            .Returns(pedidoDtos);

        var result = await _sut.GetByClienteId(clienteId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_DeveRetornarPedido_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var pedido = new Pedido { Id = id, ValorTotal = 100m };
        var pedidoDto = new PedidoDto { Id = id, ValorTotal = 100m };

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(pedido);
        _mapperMock
            .Setup(x => x.Map<PedidoDto>(pedido))
            .Returns(pedidoDto);

        var result = await _sut.GetById(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_DeveRetornarErro_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Pedido?)null);

        var result = await _sut.GetById(id);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Create_DeveRetornarErro_QuandoListaItensVazia()
    {
        var request = new CreatePedidoRequest
        {
            ClienteId = Guid.NewGuid(),
            VendedorId = Guid.NewGuid(),
            Itens = new List<CreatePedidoItemRequest>()
        };

        var result = await _sut.Create(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("item é obrigatório"));
    }

    [Fact]
    public async Task Create_DeveRetornarErro_QuandoProdutoNaoExistir()
    {
        var produtoId = Guid.NewGuid();
        var request = new CreatePedidoRequest
        {
            ClienteId = Guid.NewGuid(),
            VendedorId = Guid.NewGuid(),
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = produtoId, Quantidade = 1 }
            }
        };

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(produtoId))
            .ReturnsAsync((Produto?)null);

        var result = await _sut.Create(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Create_DeveCriarPedido_QuandoDadosValidos()
    {
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Nome = "Produto 1", Preco = 10m };
        var request = new CreatePedidoRequest
        {
            ClienteId = Guid.NewGuid(),
            VendedorId = Guid.NewGuid(),
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = produtoId, Quantidade = 2 }
            }
        };
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteId = request.ClienteId,
            VendedorId = request.VendedorId,
            ValorTotal = 20m,
            Status = StatusPedido.Pendente
        };
        var pedidoDto = new PedidoDto { Id = pedido.Id, ValorTotal = 20m };

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(produtoId))
            .ReturnsAsync(produto);
        _mapperMock
            .Setup(x => x.Map<PedidoDto>(It.IsAny<Pedido>()))
            .Returns(pedidoDto);
        _pedidoRepositoryMock
            .Setup(x => x.Add(It.IsAny<Pedido>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Create(request);

        result.IsSuccess.Should().BeTrue();
        _pedidoRepositoryMock.Verify(x => x.Add(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornarErro_QuandoPedidoNaoExistir()
    {
        var id = Guid.NewGuid();

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Pedido?)null);

        var result = await _sut.UpdateStatus(id, StatusPedido.Pago);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task UpdateStatus_DeveAtualizarStatus_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var pedido = new Pedido { Id = id, Status = StatusPedido.Pendente };
        var pedidoDto = new PedidoDto { Id = id, Status = StatusPedido.Pago };

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(pedido);
        _mapperMock
            .Setup(x => x.Map<PedidoDto>(pedido))
            .Returns(pedidoDto);
        _pedidoRepositoryMock
            .Setup(x => x.Update(It.IsAny<Pedido>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.UpdateStatus(id, StatusPedido.Pago);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(StatusPedido.Pago);
    }

    [Fact]
    public async Task Delete_DeveRetornarErro_QuandoPedidoNaoExistir()
    {
        var id = Guid.NewGuid();

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Pedido?)null);

        var result = await _sut.Delete(id);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Delete_DeveExcluirPedido_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var pedido = new Pedido { Id = id, ValorTotal = 100m };

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(pedido);
        _pedidoRepositoryMock
            .Setup(x => x.Delete(It.IsAny<Pedido>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Delete(id);

        result.IsSuccess.Should().BeTrue();
        _pedidoRepositoryMock.Verify(x => x.Delete(pedido), Times.Once);
    }
}
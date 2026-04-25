using AutoMapper;
using Crm.Application.DTOs.Checkout;
using Crm.Application.DTOs.Pedido;
using Crm.Application.Services.Implementations;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Crm.Application.Tests.Services;

public class CheckoutServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<ILinkPagamentoRepository> _linkPagamentoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly CheckoutService _sut;

    public CheckoutServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _linkPagamentoRepositoryMock = new Mock<ILinkPagamentoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(x => x["Checkout:UrlBase"]).Returns("http://localhost:5173/checkout");

        _sut = new CheckoutService(
            _pedidoRepositoryMock.Object,
            _linkPagamentoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task CriarLinkPagamento_DeveRetornarErro_QuandoPedidoNaoExistir()
    {
        var pedidoId = Guid.NewGuid();
        var request = new CreateLinkPagamentoRequest { PedidoId = pedidoId, DiasValidade = 7 };

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync((Pedido?)null);

        var result = await _sut.CriarLinkPagamento(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task CriarLinkPagamento_DeveCriarLink_QuandoPedidoExistir()
    {
        var pedidoId = Guid.NewGuid();
        var pedido = new Pedido { Id = pedidoId, ValorTotal = 100m };
        var request = new CreateLinkPagamentoRequest { PedidoId = pedidoId, DiasValidade = 7 };
        var linkPagamento = new LinkPagamento
        {
            Id = Guid.NewGuid(),
            PedidoId = pedidoId,
            Url = $"http://localhost:5173/checkout/{pedidoId}"
        };
        var linkPagamentoDto = new LinkPagamentoDto
        {
            Id = linkPagamento.Id,
            PedidoId = pedidoId,
            Url = linkPagamento.Url
        };

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedido);
        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync((LinkPagamento?)null);
        _mapperMock
            .Setup(x => x.Map<LinkPagamentoDto>(It.IsAny<LinkPagamento>()))
            .Returns(linkPagamentoDto);
        _linkPagamentoRepositoryMock
            .Setup(x => x.Add(It.IsAny<LinkPagamento>()));
        _pedidoRepositoryMock
            .Setup(x => x.Update(It.IsAny<Pedido>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.CriarLinkPagamento(request);

        result.IsSuccess.Should().BeTrue();
        _linkPagamentoRepositoryMock.Verify(x => x.Add(It.IsAny<LinkPagamento>()), Times.Once);
    }

    [Fact]
    public async Task CriarLinkPagamento_DeveAtualizarLinkExistente_QuandoJaExistir()
    {
        var pedidoId = Guid.NewGuid();
        var pedido = new Pedido { Id = pedidoId, ValorTotal = 100m };
        var linkExistente = new LinkPagamento
        {
            Id = Guid.NewGuid(),
            PedidoId = pedidoId,
            Url = "http://old.url"
        };
        var request = new CreateLinkPagamentoRequest { PedidoId = pedidoId, DiasValidade = 7 };
        var linkPagamentoDto = new LinkPagamentoDto { Id = linkExistente.Id };

        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedido);
        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync(linkExistente);
        _mapperMock
            .Setup(x => x.Map<LinkPagamentoDto>(linkExistente))
            .Returns(linkPagamentoDto);
        _linkPagamentoRepositoryMock
            .Setup(x => x.Update(It.IsAny<LinkPagamento>()));
        _pedidoRepositoryMock
            .Setup(x => x.Update(It.IsAny<Pedido>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.CriarLinkPagamento(request);

        result.IsSuccess.Should().BeTrue();
        _linkPagamentoRepositoryMock.Verify(x => x.Update(It.IsAny<LinkPagamento>()), Times.Once);
    }

    [Fact]
    public async Task GetLinkPagamento_DeveRetornarErro_QuandoNaoExistir()
    {
        var pedidoId = Guid.NewGuid();

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync((LinkPagamento?)null);

        var result = await _sut.GetLinkPagamento(pedidoId);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task GetLinkPagamento_DeveRetornarLink_QuandoExistir()
    {
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId };
        var linkPagamentoDto = new LinkPagamentoDto { Id = linkPagamento.Id, PedidoId = pedidoId };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync(linkPagamento);
        _mapperMock
            .Setup(x => x.Map<LinkPagamentoDto>(linkPagamento))
            .Returns(linkPagamentoDto);

        var result = await _sut.GetLinkPagamento(pedidoId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(linkPagamento.Id);
    }

    [Fact]
    public async Task MarcarUtilizado_DeveRetornarErro_QuandoLinkNaoExistir()
    {
        var pedidoId = Guid.NewGuid();

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync((LinkPagamento?)null);

        var result = await _sut.MarcarUtilizado(pedidoId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task MarcarUtilizado_DeveMarcarComoUtilizado_QuandoExistir()
    {
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId, Utilizado = false };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync(linkPagamento);
        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedido);
        _linkPagamentoRepositoryMock
            .Setup(x => x.Update(It.IsAny<LinkPagamento>()));
        _pedidoRepositoryMock
            .Setup(x => x.Update(It.IsAny<Pedido>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.MarcarUtilizado(pedidoId);

        result.IsSuccess.Should().BeTrue();
        linkPagamento.Utilizado.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Pago);
    }

    [Fact]
    public async Task GetPedidoParaCheckout_DeveRetornarErro_QuandoLinkNaoExistir()
    {
        var pedidoId = Guid.NewGuid();

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync((LinkPagamento?)null);

        var result = await _sut.GetPedidoParaCheckout(pedidoId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetPedidoParaCheckout_DeveRetornarErro_QuandoLinkJaUtilizado()
    {
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId, Utilizado = true };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync(linkPagamento);

        var result = await _sut.GetPedidoParaCheckout(pedidoId);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("já foi utilizado"));
    }

    [Fact]
    public async Task GetPedidoParaCheckout_DeveRetornarPedido_QuandoLinkValido()
    {
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId, Utilizado = false };
        var pedido = new Pedido { Id = pedidoId, ValorTotal = 100m };
        var pedidoDto = new PedidoDto { Id = pedidoId, ValorTotal = 100m };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPedidoIdAsync(pedidoId))
            .ReturnsAsync(linkPagamento);
        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedido);
        _mapperMock
            .Setup(x => x.Map<PedidoDto>(pedido))
            .Returns(pedidoDto);

        var result = await _sut.GetPedidoParaCheckout(pedidoId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(pedidoId);
    }
}
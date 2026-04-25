using Crm.Application.DTOs.Webhook;
using Crm.Application.Services.Implementations;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentAssertions;
using FluentResults;
using Moq;
using Xunit;

namespace Crm.Application.Tests.Services;

public class WebhookServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<ILinkPagamentoRepository> _linkPagamentoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly WebhookService _sut;

    public WebhookServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _linkPagamentoRepositoryMock = new Mock<ILinkPagamentoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new WebhookService(
            _pedidoRepositoryMock.Object,
            _linkPagamentoRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveRetornarErro_QuandoPaymentIdVazio()
    {
        var request = new WebhookRequest { PaymentId = "" };

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("obrigatório"));
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveRetornarSucesso_QuandoEventoNaoForPaymentCompleted()
    {
        var request = new WebhookRequest { PaymentId = "123", Event = "other.event" };

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveRetornarErro_QuandoLinkNaoExistir()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed" };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
            .ReturnsAsync((LinkPagamento?)null);

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveRetornarErro_QuandoPedidoNaoExistir()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed" };
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = Guid.NewGuid() };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
            .ReturnsAsync(linkPagamento);
        _pedidoRepositoryMock
            .Setup(x => x.GetByIdAsync(linkPagamento.PedidoId))
            .ReturnsAsync((Pedido?)null);

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveMarcarComoPago_QuandoStatusApproved()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed", Status = "approved" };
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
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

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsSuccess.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Pago);
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveMarcarComoPago_QuandoStatusPaid()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed", Status = "paid" };
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
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

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsSuccess.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Pago);
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveMarcarComoProcessando_QuandoStatusProcessing()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed", Status = "processing" };
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
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

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsSuccess.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Processando);
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveMarcarComoCancelado_QuandoStatusFailed()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed", Status = "failed" };
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
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

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsSuccess.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveMarcarComoCancelado_QuandoStatusCancelled()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed", Status = "cancelled" };
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
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

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsSuccess.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveManterStatus_QuandoStatusDesconhecido()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed", Status = "unknown" };
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
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

        var result = await _sut.ProcessarPagamentoWebhook(request);

        result.IsSuccess.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Pendente);
    }

    [Fact]
    public async Task ProcessarPagamentoWebhook_DeveMarcarLinkComoUtilizado()
    {
        var request = new WebhookRequest { PaymentId = "payment123", Event = "payment.completed", Status = "approved" };
        var pedidoId = Guid.NewGuid();
        var linkPagamento = new LinkPagamento { Id = Guid.NewGuid(), PedidoId = pedidoId, Utilizado = false };
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };

        _linkPagamentoRepositoryMock
            .Setup(x => x.GetByPaymentIdAsync(request.PaymentId))
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

        await _sut.ProcessarPagamentoWebhook(request);

        linkPagamento.Utilizado.Should().BeTrue();
    }
}
using Crm.Application.DTOs.Webhook;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentResults;

namespace Crm.Application.Services.Implementations;

public class WebhookService : IWebhookService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ILinkPagamentoRepository _linkPagamentoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WebhookService(
        IPedidoRepository pedidoRepository,
        ILinkPagamentoRepository linkPagamentoRepository,
        IUnitOfWork unitOfWork)
    {
        _pedidoRepository = pedidoRepository;
        _linkPagamentoRepository = linkPagamentoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ProcessarPagamentoWebhook(WebhookRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PaymentId))
            return Result.Fail(new Error("PaymentId é obrigatório."));

        if (request.Event != "payment.completed")
            return Result.Ok();

        var linkPagamento = await _linkPagamentoRepository.GetByPaymentIdAsync(request.PaymentId);
        if (linkPagamento is null)
            return Result.Fail(new Error($"Link de pagamento com PaymentId {request.PaymentId} não encontrado."));

        var pedido = await _pedidoRepository.GetByIdAsync(linkPagamento.PedidoId);
        if (pedido is null)
            return Result.Fail(new Error($"Pedido {linkPagamento.PedidoId} não encontrado."));

        linkPagamento.Utilizado = true;
        _linkPagamentoRepository.Update(linkPagamento);

        pedido.Status = request.Status?.ToLower() switch
        {
            "approved" or "paid" => StatusPedido.Pago,
            "processing" => StatusPedido.Processando,
            "failed" or "cancelled" => StatusPedido.Cancelado,
            _ => pedido.Status
        };
        _pedidoRepository.Update(pedido);

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
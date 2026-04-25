using Crm.Application.DTOs.Checkout;
using Crm.Application.DTOs.Pedido;
using FluentResults;

namespace Crm.Application.Services.Interfaces;

public interface ICheckoutService
{
    Task<Result<LinkPagamentoDto>> CriarLinkPagamento(CreateLinkPagamentoRequest request);
    Task<Result<LinkPagamentoDto>> GetLinkPagamento(Guid pedidoId);
    Task<Result> MarcarUtilizado(Guid pedidoId);
    Task<Result<PedidoDto>> GetPedidoParaCheckout(Guid pedidoId);
}
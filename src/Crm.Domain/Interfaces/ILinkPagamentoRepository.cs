using Crm.Domain.Entities;

namespace Crm.Domain.Interfaces;

public interface ILinkPagamentoRepository
{
    Task<LinkPagamento?> GetByPedidoIdAsync(Guid pedidoId);
    Task<LinkPagamento?> GetByPaymentIdAsync(string paymentId);
    void Add(LinkPagamento linkPagamento);
    void Update(LinkPagamento linkPagamento);
}
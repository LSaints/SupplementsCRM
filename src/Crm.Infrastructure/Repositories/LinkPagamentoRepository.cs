using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Repositories;

public class LinkPagamentoRepository : ILinkPagamentoRepository
{
    private readonly CrmDbContext _context;

    public LinkPagamentoRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<LinkPagamento?> GetByPedidoIdAsync(Guid pedidoId)
    {
        return await _context.LinksPagamento
            .FirstOrDefaultAsync(x => x.PedidoId == pedidoId && !x.Utilizado);
    }

    public async Task<LinkPagamento?> GetByPaymentIdAsync(string paymentId)
    {
        return await _context.LinksPagamento
            .FirstOrDefaultAsync(x => x.PaymentId == paymentId);
    }

    public void Add(LinkPagamento linkPagamento) => _context.LinksPagamento.Add(linkPagamento);
    public void Update(LinkPagamento linkPagamento) => _context.LinksPagamento.Update(linkPagamento);
}
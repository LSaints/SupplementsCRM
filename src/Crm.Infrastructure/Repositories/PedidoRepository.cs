using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly CrmDbContext _context;

    public PedidoRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> GetByIdAsync(Guid id)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(x => x.Itens)
            .ThenInclude(x => x.Produto)
            .Include(x => x.Cliente)
            .Include(x => x.Vendedor)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Pedido>> GetAllAsync()
    {
        return await _context.Pedidos
            .Include(x => x.Cliente)
            .Include(x => x.Vendedor)
            .OrderBy(x => x.CriadoEm)
            .ToListAsync();
    }

    public async Task<List<Pedido>> GetByVendedorIdAsync(Guid vendedorId)
    {
        return await _context.Pedidos
            .Where(x => x.VendedorId == vendedorId)
            .Include(x => x.Cliente)
            .OrderBy(x => x.CriadoEm)
            .ToListAsync();
    }

    public async Task<List<Pedido>> GetByClienteIdAsync(Guid clienteId)
    {
        return await _context.Pedidos
            .Where(x => x.ClienteId == clienteId)
            .Include(x => x.Itens)
            .ThenInclude(x => x.Produto)
            .OrderBy(x => x.CriadoEm)
            .ToListAsync();
    }

    public void Add(Pedido pedido) => _context.Pedidos.Add(pedido);
    public void Update(Pedido pedido) => _context.Pedidos.Update(pedido);
    public void Delete(Pedido pedido) => _context.Pedidos.Remove(pedido);
    public void RemoveItens(List<PedidoItem> itens) => _context.PedidoItens.RemoveRange(itens);
    public void RemoveItem(PedidoItem item) => _context.PedidoItens.Remove(item);
}
using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly CrmDbContext _context;

    public ClienteRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(Guid id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.Include(x => x.Vendedor).OrderBy(x => x.Nome).ToListAsync();
    }

    public async Task<List<Cliente>> GetByVendedorIdAsync(Guid vendedorId)
    {
        return await _context.Clientes
            .Where(x => x.VendedorId == vendedorId)
            .OrderBy(x => x.Nome)
            .ToListAsync();
    }

    public void Add(Cliente cliente) => _context.Clientes.Add(cliente);
    public void Update(Cliente cliente) => _context.Clientes.Update(cliente);
    public void Delete(Cliente cliente) => _context.Clientes.Remove(cliente);
}
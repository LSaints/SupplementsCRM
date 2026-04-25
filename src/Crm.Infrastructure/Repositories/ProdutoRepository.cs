using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly CrmDbContext _context;

    public ProdutoRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> GetByIdAsync(Guid id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<List<Produto>> GetAllAsync(bool incluirInativos = false)
    {
        var query = _context.Produtos.AsQueryable();
        
        if (!incluirInativos)
            query = query.Where(x => x.Ativo);

        return await query.OrderBy(x => x.Nome).ToListAsync();
    }

    public void Add(Produto produto) => _context.Produtos.Add(produto);
    public void Update(Produto produto) => _context.Produtos.Update(produto);
    public void Delete(Produto produto) => _context.Produtos.Remove(produto);
}
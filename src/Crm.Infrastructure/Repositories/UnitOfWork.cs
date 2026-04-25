using Crm.Domain.Interfaces;
using Crm.Infrastructure.Data;

namespace Crm.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CrmDbContext _context;

    public UnitOfWork(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
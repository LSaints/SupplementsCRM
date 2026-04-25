using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly CrmDbContext _context;

    public UsuarioRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(Guid id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios.OrderBy(x => x.Nome).ToListAsync();
    }

    public void Add(Usuario usuario) => _context.Usuarios.Add(usuario);
    public void Update(Usuario usuario) => _context.Usuarios.Update(usuario);
    public void Delete(Usuario usuario) => _context.Usuarios.Remove(usuario);
}
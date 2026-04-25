using Microsoft.EntityFrameworkCore;
using Crm.Domain.Entities;

namespace Crm.Infrastructure.Data;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();
    public DbSet<LinkPagamento> LinksPagamento => Set<LinkPagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);
    }
}
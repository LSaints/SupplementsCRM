using Crm.Domain.Interfaces;
using Crm.Infrastructure.Data;
using Crm.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (connectionString.Contains(":memory:") || connectionString.EndsWith(".db"))
        {
            services.AddDbContext<CrmDbContext>(options =>
                options.UseSqlite(connectionString));
        }
        else
        {
            services.AddDbContext<CrmDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<ILinkPagamentoRepository, LinkPagamentoRepository>();

        return services;
    }
}
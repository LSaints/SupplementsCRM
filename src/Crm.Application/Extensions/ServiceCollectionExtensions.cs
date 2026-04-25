using Crm.Application.Mappings;
using Crm.Application.Services.Implementations;
using Crm.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IWebhookService, WebhookService>();

        return services;
    }
}
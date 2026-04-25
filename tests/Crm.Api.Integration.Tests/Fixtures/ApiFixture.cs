using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Crm.Api;
using Crm.Infrastructure.Data;
using Microsoft.AspNetCore.TestHost;

public class ApiFactory : WebApplicationFactory<Program>
{
    public const string JwtKey = "sua_chave_secreta_minimo_32_caracteres";
    public const string JwtIssuer = "Crm";
    public const string JwtAudience = "Crm";

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"crm_test_{Process.GetCurrentProcess().Id}.db");
    private static readonly object _dbLock = new();
    private static bool _dbInitialized = false;

    public static string DbPath => _dbPath;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.UseSetting("ConnectionStrings__DefaultConnection", $"Data Source={_dbPath}");
        builder.UseSetting("Jwt:Key", JwtKey);
        builder.UseSetting("Jwt:Issuer", JwtIssuer);
        builder.UseSetting("Jwt:Audience", JwtAudience);

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<CrmDbContext>(options =>
            {
                options.UseSqlite($"Data Source={_dbPath}");
            });
        });
    }

    public HttpClient CreateUnauthenticatedClient()
    {
        return CreateClient();
    }

    public HttpClient CreateAuthenticatedClient(string role = "Admin")
    {
        var token = AuthHelper.GenerateJwtToken(role: role);
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
            return default;

        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    public static string JwtKeyConst => JwtKey;
    public static string JwtIssuerConst => JwtIssuer;
    public static string JwtAudienceConst => JwtAudience;

    public static void ResetDatabase()
    {
        _dbInitialized = false;
        try { File.Delete(_dbPath); } catch { }
        try { File.Delete(_dbPath + "-wal"); } catch { }
        try { File.Delete(_dbPath + "-shm"); } catch { }
    }
}

public class ApiFixture : IAsyncLifetime
{
    private ApiFactory? _factory;

    public ApiFactory Factory => _factory ?? throw new InvalidOperationException("Fixture not initialized");

    public async Task InitializeAsync()
    {
        _factory = new ApiFactory();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        ApiFactory.ResetDatabase();
        return Task.CompletedTask;
    }

    public void ClearAndReset()
    {
        if (_factory == null) return;
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        
        dbContext.PedidoItens.RemoveRange(dbContext.PedidoItens.ToList());
        dbContext.LinksPagamento.RemoveRange(dbContext.LinksPagamento.ToList());
        dbContext.Pedidos.RemoveRange(dbContext.Pedidos.ToList());
        dbContext.Clientes.RemoveRange(dbContext.Clientes.ToList());
        dbContext.Produtos.RemoveRange(dbContext.Produtos.ToList());
        dbContext.Usuarios.RemoveRange(dbContext.Usuarios.ToList());
        dbContext.SaveChanges();
    }
}

[CollectionDefinition("ApiFixture")]
public class ApiFixtureCollection : ICollectionFixture<ApiFixture> { }

public static class AuthHelper
{
    public static string GenerateJwtToken(string email = "test@example.com", string role = "Admin")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFactory.JwtKeyConst));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: ApiFactory.JwtIssuerConst,
            audience: ApiFactory.JwtAudienceConst,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}
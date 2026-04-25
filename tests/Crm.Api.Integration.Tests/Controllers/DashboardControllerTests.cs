using System.Net;
using System.Text;
using System.Text.Json;
using Crm.Application.DTOs.Dashboard;
using Crm.Application.DTOs.Pedido;
using FluentAssertions;
using Xunit;

namespace Crm.Api.Integration.Tests.Controllers;

[Collection("ApiFixture")]
public class DashboardControllerTests : IDisposable
{
    private readonly ApiFixture _fixture;

    public DashboardControllerTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAndReset();
    }

    public void Dispose() => _fixture.ClearAndReset();

    [Fact]
    public async Task Get_DeveRetornar401_QuandoNaoAutenticado()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_DeveRetornarDadosVazios_QuandoNaoExistiremDados()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<DashboardDto>(response);
        result!.TotalClientes.Should().Be(0);
        result.TotalPedidos.Should().Be(0);
    }

    [Fact]
    public async Task Get_DeveRetornarDadosCorretos_QuandoExistiremDados()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        await client.PostAsync("/api/clientes", CreateJsonContent(new Crm.Application.DTOs.Cliente.CreateClienteRequest
        {
            Nome = "Cliente Dashboard",
            Email = "dash@test.com"
        }));

        var clienteResponse = await client.PostAsync("/api/clientes", CreateJsonContent(new Crm.Application.DTOs.Cliente.CreateClienteRequest
        {
            Nome = "Cliente Dashboard 2",
            Email = "dash2@test.com"
        }));
        var cliente = (await ApiFactory.DeserializeAsync<Crm.Application.DTOs.Cliente.ClienteDto>(clienteResponse))!;

        var produtoResponse = await client.PostAsync("/api/produtos", CreateJsonContent(new Crm.Application.DTOs.Produto.CreateProdutoRequest
        {
            Nome = "Produto Dashboard",
            Preco = 100m
        }));
        var produto = (await ApiFactory.DeserializeAsync<Crm.Application.DTOs.Produto.ProdutoDto>(produtoResponse))!;

        await client.PostAsync("/api/pedidos", CreateJsonContent(new CreatePedidoRequest
        {
            ClienteId = cliente.Id,
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = produto.Id, Quantidade = 2 }
            }
        }));

        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<DashboardDto>(response);
        result!.TotalClientes.Should().Be(2);
        result.TotalPedidos.Should().Be(1);
    }

    private static HttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}
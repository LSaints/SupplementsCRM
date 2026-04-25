using System.Net;
using System.Text;
using System.Text.Json;
using Crm.Application.DTOs.Pedido;
using Crm.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Crm.Api.Integration.Tests.Controllers;

[Collection("ApiFixture")]
public class PedidosControllerTests : IDisposable
{
    private readonly ApiFixture _fixture;

    public PedidosControllerTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAndReset();
    }

    public void Dispose() => _fixture.ClearAndReset();

    [Fact]
    public async Task GetAll_DeveRetornar401_QuandoNaoAutenticado()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/pedidos");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaVazia_QuandoNaoExistiremPedidos()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/pedidos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<List<PedidoDto>>(response);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoClienteNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var request = new CreatePedidoRequest
        {
            ClienteId = Guid.NewGuid(),
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = Guid.NewGuid(), Quantidade = 1 }
            }
        };

        var response = await client.PostAsync("/api/pedidos", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.GetAsync($"/api/pedidos/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateStatus_DeveAtualizarStatus_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        var pedidoResponse = await CreatePedidoComClienteEProdutoAsync(client);
        var pedido = await ApiFactory.DeserializeAsync<PedidoDto>(pedidoResponse);

        var content = new StringContent("1", Encoding.UTF8, "application/json");
        var response = await client.PatchAsync($"/api/pedidos/{pedido!.Id}/status", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var content = new StringContent($"\"{(int)StatusPedido.Pago}\"", Encoding.UTF8, "application/json");
        var response = await client.PatchAsync($"/api/pedidos/{id}/status", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByVendedorId_DeveRetornarPedidosDoVendedor()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var vendedorId = Guid.NewGuid();
        var cliente = await CriarClienteAsync(client);
        var produto = await CriarProdutoAsync(client);

        await client.PostAsync("/api/pedidos", CreateJsonContent(new CreatePedidoRequest
        {
            ClienteId = cliente.Id,
            VendedorId = vendedorId,
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = produto.Id, Quantidade = 2 }
            }
        }));

        var response = await client.GetAsync($"/api/pedidos/vendedor/{vendedorId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByClienteId_DeveRetornarPedidosDoCliente()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var cliente = await CriarClienteAsync(client);
        var produto = await CriarProdutoAsync(client);

        await client.PostAsync("/api/pedidos", CreateJsonContent(new CreatePedidoRequest
        {
            ClienteId = cliente.Id,
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = produto.Id, Quantidade = 1 }
            }
        }));

        var response = await client.GetAsync($"/api/pedidos/cliente/{cliente.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_DeveExcluirPedido_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var pedidoResponse = await CreatePedidoComClienteEProdutoAsync(client);
        var pedido = await ApiFactory.DeserializeAsync<PedidoDto>(pedidoResponse);

        var response = await client.DeleteAsync($"/api/pedidos/{pedido!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<HttpResponseMessage> CreatePedidoComClienteEProdutoAsync(HttpClient client)
    {
        var cliente = await CriarClienteAsync(client);
        var produto = await CriarProdutoAsync(client);

        return await client.PostAsync("/api/pedidos", CreateJsonContent(new CreatePedidoRequest
        {
            ClienteId = cliente.Id,
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = produto.Id, Quantidade = 1 }
            }
        }));
    }

    private async Task<Crm.Application.DTOs.Cliente.ClienteDto> CriarClienteAsync(HttpClient client)
    {
        var response = await client.PostAsync("/api/clientes", CreateJsonContent(new Crm.Application.DTOs.Cliente.CreateClienteRequest
        {
            Nome = "Cliente Teste",
            Email = "teste@test.com"
        }));
        return (await ApiFactory.DeserializeAsync<Crm.Application.DTOs.Cliente.ClienteDto>(response))!;
    }

    private async Task<Crm.Application.DTOs.Produto.ProdutoDto> CriarProdutoAsync(HttpClient client)
    {
        var response = await client.PostAsync("/api/produtos", CreateJsonContent(new Crm.Application.DTOs.Produto.CreateProdutoRequest
        {
            Nome = "Produto Teste",
            Preco = 10m
        }));
        return (await ApiFactory.DeserializeAsync<Crm.Application.DTOs.Produto.ProdutoDto>(response))!;
    }

    private static HttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, ApiFactory.JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}
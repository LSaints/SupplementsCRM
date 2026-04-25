using System.Net;
using System.Text;
using System.Text.Json;
using Crm.Application.DTOs.Checkout;
using Crm.Application.DTOs.Pedido;
using FluentAssertions;
using Xunit;

namespace Crm.Api.Integration.Tests.Controllers;

[Collection("ApiFixture")]
public class CheckoutControllerTests : IDisposable
{
    private readonly ApiFixture _fixture;

    public CheckoutControllerTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAndReset();
    }

    public void Dispose() => _fixture.ClearAndReset();

    [Fact]
    public async Task CriarLinkPagamento_DeveCriarLink_QuandoPedidoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var pedido = await CriarPedidoAsync(client);

        var request = new CreateLinkPagamentoRequest
        {
            PedidoId = pedido.Id,
            DiasValidade = 5
        };
        var response = await client.PostAsync("/api/checkout/link", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<LinkPagamentoDto>(response);
        result!.PedidoId.Should().Be(pedido.Id);
        result.Url.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CriarLinkPagamento_DeveRetornar400_QuandoPedidoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var request = new CreateLinkPagamentoRequest
        {
            PedidoId = Guid.NewGuid(),
            DiasValidade = 3
        };

        var response = await client.PostAsync("/api/checkout/link", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetLinkPagamento_DeveRetornarLink_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var pedido = await CriarPedidoAsync(client);

        await client.PostAsync("/api/checkout/link", CreateJsonContent(new CreateLinkPagamentoRequest
        {
            PedidoId = pedido.Id
        }));

        var response = await client.GetAsync($"/api/checkout/link/{pedido.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLinkPagamento_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.GetAsync($"/api/checkout/link/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCheckoutData_DeveRetornarDadosDoPedido_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var pedido = await CriarPedidoAsync(client);

        await client.PostAsync("/api/checkout/link", CreateJsonContent(new CreateLinkPagamentoRequest
        {
            PedidoId = pedido.Id
        }));

        var response = await client.GetAsync($"/api/checkout/{pedido.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCheckoutData_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.GetAsync($"/api/checkout/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConfirmarPagamento_DeveMarcarLinkComoUtilizado_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var pedido = await CriarPedidoAsync(client);

        await client.PostAsync("/api/checkout/link", CreateJsonContent(new CreateLinkPagamentoRequest
        {
            PedidoId = pedido.Id
        }));

        var response = await client.PostAsync($"/api/checkout/link/{pedido.Id}/confirmar", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ConfirmarPagamento_DeveRetornar400_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.PostAsync($"/api/checkout/link/{id}/confirmar", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<PedidoDto> CriarPedidoAsync(HttpClient client)
    {
        var clienteResponse = await client.PostAsync("/api/clientes", CreateJsonContent(new Crm.Application.DTOs.Cliente.CreateClienteRequest
        {
            Nome = "Cliente Checkout",
            Email = $"checkout_{Guid.NewGuid()}@test.com"
        }));

        if (!clienteResponse.IsSuccessStatusCode)
            throw new InvalidOperationException($"Falha ao criar cliente: {clienteResponse.StatusCode}");

        var cliente = (await ApiFactory.DeserializeAsync<Crm.Application.DTOs.Cliente.ClienteDto>(clienteResponse))!;

        var produtoResponse = await client.PostAsync("/api/produtos", CreateJsonContent(new Crm.Application.DTOs.Produto.CreateProdutoRequest
        {
            Nome = "Produto Checkout",
            Preco = 100m
        }));

        if (!produtoResponse.IsSuccessStatusCode)
            throw new InvalidOperationException($"Falha ao criar produto: {produtoResponse.StatusCode}");

        var produto = (await ApiFactory.DeserializeAsync<Crm.Application.DTOs.Produto.ProdutoDto>(produtoResponse))!;

        var pedidoResponse = await client.PostAsync("/api/pedidos", CreateJsonContent(new CreatePedidoRequest
        {
            ClienteId = cliente.Id,
            Itens = new List<CreatePedidoItemRequest>
            {
                new() { ProdutoId = produto.Id, Quantidade = 1 }
            }
        }));

        if (!pedidoResponse.IsSuccessStatusCode)
            throw new InvalidOperationException($"Falha ao criar pedido: {pedidoResponse.StatusCode}");

        return (await ApiFactory.DeserializeAsync<PedidoDto>(pedidoResponse))!;
    }

    private static HttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}
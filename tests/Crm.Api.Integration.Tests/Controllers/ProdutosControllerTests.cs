using System.Net;
using System.Text;
using System.Text.Json;
using Crm.Application.DTOs.Produto;
using FluentAssertions;
using Xunit;

namespace Crm.Api.Integration.Tests.Controllers;

[Collection("ApiFixture")]
public class ProdutosControllerTests : IDisposable
{
    private readonly ApiFixture _fixture;

    public ProdutosControllerTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAndReset();
    }

    public void Dispose() => _fixture.ClearAndReset();

    [Fact]
    public async Task GetAll_DeveRetornar401_QuandoNaoAutenticado()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/produtos");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaVazia_QuandoNaoExistiremProdutos()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/produtos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<List<ProdutoDto>>(response);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_DeveRetornarProdutos_QuandoExistirem()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        await client.PostAsync("/api/produtos", CreateJsonContent(new CreateProdutoRequest
        {
            Nome = "Produto 1",
            Preco = 10m,
            Descricao = "Descrição 1"
        }));

        await client.PostAsync("/api/produtos", CreateJsonContent(new CreateProdutoRequest
        {
            Nome = "Produto 2",
            Preco = 20m,
            Descricao = "Descrição 2"
        }));

        var response = await client.GetAsync("/api/produtos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<List<ProdutoDto>>(response);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_DeveCriarProduto_QuandoDadosValidos()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var request = new CreateProdutoRequest
        {
            Nome = "Novo Produto",
            Preco = 100m,
            Descricao = "Nova descrição"
        };

        var response = await client.PostAsync("/api/produtos", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await ApiFactory.DeserializeAsync<ProdutoDto>(response);
        result!.Nome.Should().Be(request.Nome);
        result.Preco.Should().Be(request.Preco);
        result.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoNomeVazio()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var request = new CreateProdutoRequest { Nome = "", Preco = 10m };

        var response = await client.PostAsync("/api/produtos", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_DeveRetornarProduto_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var createResponse = await client.PostAsync("/api/produtos", CreateJsonContent(new CreateProdutoRequest
        {
            Nome = "Produto Teste",
            Preco = 50m
        }));
        var produto = await ApiFactory.DeserializeAsync<ProdutoDto>(createResponse);

        var response = await client.GetAsync($"/api/produtos/{produto!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<ProdutoDto>(response);
        result!.Id.Should().Be(produto.Id);
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.GetAsync($"/api/produtos/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DeveAtualizarProduto_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var createResponse = await client.PostAsync("/api/produtos", CreateJsonContent(new CreateProdutoRequest
        {
            Nome = "Produto Antigo",
            Preco = 10m
        }));
        var produto = await ApiFactory.DeserializeAsync<ProdutoDto>(createResponse);

        var updateRequest = new UpdateProdutoRequest
        {
            Nome = "Produto Novo",
            Preco = 99m,
            Descricao = "Nova descrição",
            Ativo = false
        };
        var response = await client.PutAsync($"/api/produtos/{produto!.Id}", CreateJsonContent(updateRequest));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<ProdutoDto>(response);
        result!.Nome.Should().Be("Produto Novo");
        result.Preco.Should().Be(99m);
    }

    [Fact]
    public async Task Update_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.PutAsync($"/api/produtos/{id}", CreateJsonContent(new UpdateProdutoRequest
        {
            Nome = "Teste",
            Preco = 10m
        }));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveExcluirProduto_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var createResponse = await client.PostAsync("/api/produtos", CreateJsonContent(new CreateProdutoRequest
        {
            Nome = "Produto para Deletar",
            Preco = 10m
        }));
        var produto = await ApiFactory.DeserializeAsync<ProdutoDto>(createResponse);

        var response = await client.DeleteAsync($"/api/produtos/{produto!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.DeleteAsync($"/api/produtos/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_DeveRetornarInativos_QuandoIncluirInativosTrue()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var createResponse = await client.PostAsync("/api/produtos", CreateJsonContent(new CreateProdutoRequest
        {
            Nome = "Produto Ativo",
            Preco = 10m
        }));
        var produto = await ApiFactory.DeserializeAsync<ProdutoDto>(createResponse);

        await client.PutAsync($"/api/produtos/{produto!.Id}", CreateJsonContent(new UpdateProdutoRequest
        {
            Nome = "Produto Inativo",
            Preco = 10m,
            Ativo = false
        }));

        var response = await client.GetAsync("/api/produtos?incluirInativos=true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<List<ProdutoDto>>(response);
        result.Should().HaveCount(1);
    }

    private static HttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}
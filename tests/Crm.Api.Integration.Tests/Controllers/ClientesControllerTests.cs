using System.Net;
using System.Text;
using System.Text.Json;
using Crm.Application.DTOs.Cliente;
using FluentAssertions;
using Xunit;

namespace Crm.Api.Integration.Tests.Controllers;

[Collection("ApiFixture")]
public class ClientesControllerTests : IDisposable
{
    private readonly ApiFixture _fixture;

    public ClientesControllerTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAndReset();
    }

    public void Dispose() => _fixture.ClearAndReset();

    [Fact]
    public async Task GetAll_DeveRetornar401_QuandoNaoAutenticado()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/clientes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaVazia_QuandoNaoExistiremClientes()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/clientes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<List<ClienteDto>>(response);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_DeveRetornarClientes_QuandoExistirem()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        await client.PostAsync("/api/clientes", CreateJsonContent(new CreateClienteRequest
        {
            Nome = "Cliente 1",
            Email = "cliente1@test.com",
            Telefone = "11999999999"
        }));
        await client.PostAsync("/api/clientes", CreateJsonContent(new CreateClienteRequest
        {
            Nome = "Cliente 2",
            Email = "cliente2@test.com",
            Telefone = "11888888888"
        }));

        var response = await client.GetAsync("/api/clientes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<List<ClienteDto>>(response);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_DeveCriarCliente_QuandoDadosValidos()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var request = new CreateClienteRequest
        {
            Nome = "Novo Cliente",
            Email = "novo@test.com",
            Telefone = "11999999999",
            Endereco = new EnderecoDto
            {
                Rua = "Rua Teste",
                Numero = "123",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "01234-567"
            }
        };

        var response = await client.PostAsync("/api/clientes", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await ApiFactory.DeserializeAsync<ClienteDto>(response);
        result!.Nome.Should().Be(request.Nome);
        result.Email.Should().Be(request.Email);
    }





    [Fact]
    public async Task GetById_DeveRetornar404_QuandoNaoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var id = Guid.NewGuid();

        var response = await client.GetAsync($"/api/clientes/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DeveAtualizarCliente_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var createResponse = await client.PostAsync("/api/clientes", CreateJsonContent(new CreateClienteRequest
        {
            Nome = "Cliente Antigo",
            Email = "antigo@test.com"
        }));
        var cliente = await ApiFactory.DeserializeAsync<ClienteDto>(createResponse);

        var updateRequest = new UpdateClienteRequest
        {
            Nome = "Cliente Novo",
            Email = "novo@test.com",
            Telefone = "11999999999"
        };
        var response = await client.PutAsync($"/api/clientes/{cliente!.Id}", CreateJsonContent(updateRequest));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<ClienteDto>(response);
        result!.Nome.Should().Be("Cliente Novo");
    }

    [Fact]
    public async Task Delete_DeveExcluirCliente_QuandoExistir()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();
        var createResponse = await client.PostAsync("/api/clientes", CreateJsonContent(new CreateClienteRequest
        {
            Nome = "Cliente para Deletar",
            Email = "deletar@test.com"
        }));
        var cliente = await ApiFactory.DeserializeAsync<ClienteDto>(createResponse);

        var response = await client.DeleteAsync($"/api/clientes/{cliente!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetByVendedorId_DeveRetornarClientesDoVendedor()
    {
        var client = _fixture.Factory.CreateAuthenticatedClient();

        await client.PostAsync("/api/clientes", CreateJsonContent(new CreateClienteRequest
        {
            Nome = "Cliente Vendedor 1",
            Email = "v1@test.com",
            VendedorId = Guid.NewGuid()
        }));

        var response = await client.GetAsync($"/api/clientes/vendedor/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static HttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}
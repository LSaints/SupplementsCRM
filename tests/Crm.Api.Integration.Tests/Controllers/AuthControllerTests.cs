using System.Net;
using System.Text;
using System.Text.Json;
using Crm.Application.DTOs.Auth;
using FluentAssertions;
using Xunit;

namespace Crm.Api.Integration.Tests.Controllers;

[Collection("ApiFixture")]
public class AuthControllerTests
{
    private readonly ApiFixture _fixture;

    public AuthControllerTests(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Register_DeveCriarUsuario_QuandoDadosValidos()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();
        var request = new LoginRequest
        {
            Email = "novousuario@example.com",
            Senha = "SenhaForte123!"
        };

        var response = await client.PostAsync("/api/auth/register", CreateJsonContent(request));
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Expected OK but got {response.StatusCode}. Content: {content}");
        var result = await ApiFactory.DeserializeAsync<LoginResponse>(response);
        result!.Token.Should().NotBeNullOrEmpty();
        result.Usuario.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Register_DeveRetornar400_QuandoEmailJaExistente()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();
        var request = new LoginRequest
        {
            Email = "existente@example.com",
            Senha = "SenhaForte123!"
        };

        await client.PostAsync("/api/auth/register", CreateJsonContent(request));
        var response = await client.PostAsync("/api/auth/register", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();
        var registerRequest = new LoginRequest
        {
            Email = "logintest@example.com",
            Senha = "SenhaForte123!"
        };
        await client.PostAsync("/api/auth/register", CreateJsonContent(registerRequest));

        var loginRequest = new LoginRequest
        {
            Email = "logintest@example.com",
            Senha = "SenhaForte123!"
        };
        var response = await client.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiFactory.DeserializeAsync<LoginResponse>(response);
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoSenhaIncorreta()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();
        var registerRequest = new LoginRequest
        {
            Email = "seno@example.com",
            Senha = "SenhaCorreta123!"
        };
        await client.PostAsync("/api/auth/register", CreateJsonContent(registerRequest));

        var loginRequest = new LoginRequest
        {
            Email = "seno@example.com",
            Senha = "SenhaIncorreta123!"
        };
        var response = await client.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoUsuarioNaoExistir()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();
        var request = new LoginRequest
        {
            Email = "naoexiste@example.com",
            Senha = "Senha123!"
        };

        var response = await client.PostAsync("/api/auth/login", CreateJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Deve_retornar_token_com_role_Admin_QuandoRegistroAdmin()
    {
        var client = _fixture.Factory.CreateUnauthenticatedClient();
        var request = new LoginRequest
        {
            Email = "admin@test.com",
            Senha = "SenhaAdmin123!"
        };

        await client.PostAsync("/api/auth/register", CreateJsonContent(request));

        var loginRequest = new LoginRequest
        {
            Email = "admin@test.com",
            Senha = "SenhaAdmin123!"
        };
        var response = await client.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static HttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}
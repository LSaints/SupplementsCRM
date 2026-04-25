using AutoMapper;
using Crm.Application.DTOs.Auth;
using Crm.Application.Services.Implementations;
using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Crm.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();
        
        var configurationSectionMock = new Mock<IConfigurationSection>();
        configurationSectionMock.Setup(x => x.Value).Returns("chave_secreta_muito_segura_minimo_32_caracteres!");
        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("chave_secreta_muito_segura_minimo_32_caracteres!");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("Crm");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("Crm");
        
        _sut = new AuthService(
            _usuarioRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Login_DeveRetornarErro_QuandoUsuarioNaoExistir()
    {
        var request = new LoginRequest { Email = "teste@email.com", Senha = "senha123" };

        _usuarioRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        var result = await _sut.Login(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("inválidos"));
    }

    [Fact]
    public async Task Login_DeveRetornarErro_QuandoSenhaIncorreta()
    {
        var request = new LoginRequest { Email = "teste@email.com", Senha = "senha_errada" };
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha_correta"),
            Role = "Vendedor"
        };

        _usuarioRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        var result = await _sut.Login(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("inválidos"));
    }

    [Fact]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var request = new LoginRequest { Email = "teste@email.com", Senha = "senha_correta" };
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha_correta"),
            Role = "Vendedor"
        };
        var usuarioDto = new UsuarioDto
        {
            Id = usuario.Id,
            Nome = "Teste",
            Email = "teste@email.com",
            Role = "Vendedor"
        };

        _usuarioRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(usuario);
        _mapperMock
            .Setup(x => x.Map<UsuarioDto>(usuario))
            .Returns(usuarioDto);

        var result = await _sut.Login(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().NotBeEmpty();
        result.Value.Usuario.Email.Should().Be("teste@email.com");
    }

    [Fact]
    public async Task Register_DeveRetornarErro_QuandoEmailJaCadastrado()
    {
        var request = new LoginRequest { Email = "teste@email.com", Senha = "senha123" };
        var usuarioExistente = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash_existente"
        };

        _usuarioRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(usuarioExistente);

        var result = await _sut.Register(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("cadastrado"));
    }

    [Fact]
    public async Task Register_DeveCriarUsuario_QuandoEmailNaoExistir()
    {
        var request = new LoginRequest { Email = "novo@email.com", Senha = "senha123" };
        var usuarioCriado = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "novo",
            Email = "novo@email.com",
            Role = "Vendedor"
        };
        var usuarioDto = new UsuarioDto
        {
            Id = usuarioCriado.Id,
            Nome = "novo",
            Email = "novo@email.com",
            Role = "Vendedor"
        };

        _usuarioRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);
        _mapperMock
            .Setup(x => x.Map<UsuarioDto>(It.IsAny<Usuario>()))
            .Returns(usuarioDto);
        _usuarioRepositoryMock
            .Setup(x => x.Add(It.IsAny<Usuario>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Register(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().NotBeEmpty();
        _usuarioRepositoryMock.Verify(x => x.Add(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task Register_DeveAtribuirRoleVendedor_PorPadrao()
    {
        var request = new LoginRequest { Email = "novo@email.com", Senha = "senha123" };
        Usuario? usuarioAdicionado = null;

        _usuarioRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);
        _mapperMock
            .Setup(x => x.Map<UsuarioDto>(It.IsAny<Usuario>()))
            .Returns(new UsuarioDto());
        _usuarioRepositoryMock
            .Setup(x => x.Add(It.IsAny<Usuario>()))
            .Callback<Usuario>(u => usuarioAdicionado = u);
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        await _sut.Register(request);

        usuarioAdicionado.Should().NotBeNull();
        usuarioAdicionado!.Role.Should().Be("Vendedor");
    }
}
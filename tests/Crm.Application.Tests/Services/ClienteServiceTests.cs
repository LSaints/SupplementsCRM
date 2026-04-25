using AutoMapper;
using Crm.Application.DTOs.Cliente;
using Crm.Application.Services.Implementations;
using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using Crm.Domain.ValueObjects;
using FluentAssertions;
using FluentResults;
using Moq;
using Xunit;

namespace Crm.Application.Tests.Services;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ClienteService _sut;

    public ClienteServiceTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _sut = new ClienteService(
            _clienteRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaClientes()
    {
        var clientes = new List<Cliente>
        {
            new() { Id = Guid.NewGuid(), Nome = "Cliente 1" },
            new() { Id = Guid.NewGuid(), Nome = "Cliente 2" }
        };
        var clienteDtos = new List<ClienteDto>
        {
            new() { Id = clientes[0].Id, Nome = "Cliente 1" },
            new() { Id = clientes[1].Id, Nome = "Cliente 2" }
        };

        _clienteRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(clientes);
        _mapperMock
            .Setup(x => x.Map<List<ClienteDto>>(clientes))
            .Returns(clienteDtos);

        var result = await _sut.GetAll();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByVendedorId_DeveRetornarClientesDoVendedor()
    {
        var vendedorId = Guid.NewGuid();
        var clientes = new List<Cliente>
        {
            new() { Id = Guid.NewGuid(), Nome = "Cliente 1", VendedorId = vendedorId }
        };
        var clienteDtos = new List<ClienteDto>
        {
            new() { Id = clientes[0].Id, Nome = "Cliente 1", VendedorId = vendedorId }
        };

        _clienteRepositoryMock
            .Setup(x => x.GetByVendedorIdAsync(vendedorId))
            .ReturnsAsync(clientes);
        _mapperMock
            .Setup(x => x.Map<List<ClienteDto>>(clientes))
            .Returns(clienteDtos);

        var result = await _sut.GetByVendedorId(vendedorId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_DeveRetornarCliente_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var cliente = new Cliente { Id = id, Nome = "Cliente 1" };
        var clienteDto = new ClienteDto { Id = id, Nome = "Cliente 1" };

        _clienteRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(cliente);
        _mapperMock
            .Setup(x => x.Map<ClienteDto>(cliente))
            .Returns(clienteDto);

        var result = await _sut.GetById(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_DeveRetornarErro_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();

        _clienteRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Cliente?)null);

        var result = await _sut.GetById(id);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Create_DeveRetornarErro_QuandoNomeForVazio()
    {
        var request = new CreateClienteRequest { Nome = "" };

        var result = await _sut.Create(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("obrigatório"));
    }

    [Fact]
    public async Task Create_DeveRetornarErro_QuandoNomeForEspacoEmBranco()
    {
        var request = new CreateClienteRequest { Nome = "   " };

        var result = await _sut.Create(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("obrigatório"));
    }

    [Fact]
    public async Task Create_DeveCriarCliente_QuandoDadosValidos()
    {
        var request = new CreateClienteRequest
        {
            Nome = "Cliente Novo",
            Email = "cliente@email.com",
            Telefone = "11999999999",
            VendedorId = Guid.NewGuid()
        };
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Cliente Novo",
            Email = "cliente@email.com",
            Telefone = "11999999999",
            VendedorId = request.VendedorId!.Value
        };
        var clienteDto = new ClienteDto
        {
            Id = cliente.Id,
            Nome = "Cliente Novo",
            Email = "cliente@email.com",
            Telefone = "11999999999",
            VendedorId = request.VendedorId!.Value
        };

        _mapperMock
            .Setup(x => x.Map<Cliente>(request))
            .Returns(cliente);
        _mapperMock
            .Setup(x => x.Map<ClienteDto>(cliente))
            .Returns(clienteDto);
        _clienteRepositoryMock
            .Setup(x => x.Add(It.IsAny<Cliente>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Create(request);

        result.IsSuccess.Should().BeTrue();
        _clienteRepositoryMock.Verify(x => x.Add(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task Create_DeveCriarClienteComEndereco_QuandoEnderecoFornecido()
    {
        var request = new CreateClienteRequest
        {
            Nome = "Cliente Novo",
            Endereco = new EnderecoDto
            {
                Rua = "Rua Teste",
                Numero = "123",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "01000000"
            },
            VendedorId = Guid.NewGuid()
        };
        var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente Novo" };

        _mapperMock
            .Setup(x => x.Map<Cliente>(request))
            .Returns(cliente);
        _mapperMock
            .Setup(x => x.Map<ClienteDto>(cliente))
            .Returns(new ClienteDto());
        _clienteRepositoryMock
            .Setup(x => x.Add(It.IsAny<Cliente>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Create(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Update_DeveRetornarErro_QuandoClienteNaoExistir()
    {
        var id = Guid.NewGuid();
        var request = new UpdateClienteRequest { Nome = "Cliente Atualizado" };

        _clienteRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Cliente?)null);

        var result = await _sut.Update(id, request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Update_DeveAtualizarCliente_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var cliente = new Cliente { Id = id, Nome = "Cliente Antigo" };
        var request = new UpdateClienteRequest
        {
            Nome = "Cliente Atualizado",
            Email = "atualizado@email.com",
            Telefone = "11999999999",
            VendedorId = Guid.NewGuid()
        };
        var clienteDto = new ClienteDto { Id = id, Nome = "Cliente Atualizado" };

        _clienteRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(cliente);
        _mapperMock
            .Setup(x => x.Map<ClienteDto>(cliente))
            .Returns(clienteDto);
        _clienteRepositoryMock
            .Setup(x => x.Update(It.IsAny<Cliente>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Update(id, request);

        result.IsSuccess.Should().BeTrue();
        _clienteRepositoryMock.Verify(x => x.Update(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task Delete_DeveRetornarErro_QuandoClienteNaoExistir()
    {
        var id = Guid.NewGuid();

        _clienteRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Cliente?)null);

        var result = await _sut.Delete(id);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Delete_DeveExcluirCliente_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var cliente = new Cliente { Id = id, Nome = "Cliente 1" };

        _clienteRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(cliente);
        _clienteRepositoryMock
            .Setup(x => x.Delete(It.IsAny<Cliente>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Delete(id);

        result.IsSuccess.Should().BeTrue();
        _clienteRepositoryMock.Verify(x => x.Delete(cliente), Times.Once);
    }
}
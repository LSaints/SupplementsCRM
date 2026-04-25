using AutoMapper;
using Crm.Application.DTOs.Produto;
using Crm.Application.Services.Implementations;
using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using FluentAssertions;
using FluentResults;
using Moq;
using Xunit;

namespace Crm.Application.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProdutoService _sut;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _sut = new ProdutoService(
            _produtoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaProdutos_QuandoExistirem()
    {
        var produtos = new List<Produto>
        {
            new() { Id = Guid.NewGuid(), Nome = "Produto 1", Preco = 10m, Ativo = true },
            new() { Id = Guid.NewGuid(), Nome = "Produto 2", Preco = 20m, Ativo = true }
        };
        var produtoDtos = new List<ProdutoDto>
        {
            new() { Id = produtos[0].Id, Nome = "Produto 1", Preco = 10m, Ativo = true },
            new() { Id = produtos[1].Id, Nome = "Produto 2", Preco = 20m, Ativo = true }
        };

        _produtoRepositoryMock
            .Setup(x => x.GetAllAsync(false))
            .ReturnsAsync(produtos);
        _mapperMock
            .Setup(x => x.Map<List<ProdutoDto>>(produtos))
            .Returns(produtoDtos);

        var result = await _sut.GetAll();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaVazia_QuandoNaoExistiremProdutos()
    {
        var produtos = new List<Produto>();
        var produtoDtos = new List<ProdutoDto>();

        _produtoRepositoryMock
            .Setup(x => x.GetAllAsync(false))
            .ReturnsAsync(produtos);
        _mapperMock
            .Setup(x => x.Map<List<ProdutoDto>>(produtos))
            .Returns(produtoDtos);

        var result = await _sut.GetAll();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_DeveRetornarProduto_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var produto = new Produto { Id = id, Nome = "Produto 1", Preco = 10m, Ativo = true };
        var produtoDto = new ProdutoDto { Id = id, Nome = "Produto 1", Preco = 10m, Ativo = true };

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(produto);
        _mapperMock
            .Setup(x => x.Map<ProdutoDto>(produto))
            .Returns(produtoDto);

        var result = await _sut.GetById(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_DeveRetornarErro_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Produto?)null);

        var result = await _sut.GetById(id);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Create_DeveRetornarErro_QuandoNomeForVazio()
    {
        var request = new CreateProdutoRequest { Nome = "", Preco = 10m };

        var result = await _sut.Create(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("obrigatório"));
    }

    [Fact]
    public async Task Create_DeveRetornarErro_QuandoNomeForEspacoEmBranco()
    {
        var request = new CreateProdutoRequest { Nome = "   ", Preco = 10m };

        var result = await _sut.Create(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("obrigatório"));
    }

    [Fact]
    public async Task Create_DeveCriarProduto_QuandoDadosValidos()
    {
        var request = new CreateProdutoRequest { Nome = "Produto 1", Preco = 10m, Descricao = "Descrição" };
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto 1", Preco = 10m, Descricao = "Descrição", Ativo = true };
        var produtoDto = new ProdutoDto { Id = produto.Id, Nome = "Produto 1", Preco = 10m, Descricao = "Descrição", Ativo = true };

        _mapperMock
            .Setup(x => x.Map<Produto>(request))
            .Returns(produto);
        _mapperMock
            .Setup(x => x.Map<ProdutoDto>(produto))
            .Returns(produtoDto);
        _produtoRepositoryMock
            .Setup(x => x.Add(It.IsAny<Produto>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Create(request);

        result.IsSuccess.Should().BeTrue();
        _produtoRepositoryMock.Verify(x => x.Add(It.IsAny<Produto>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Update_DeveRetornarErro_QuandoProdutoNaoExistir()
    {
        var id = Guid.NewGuid();
        var request = new UpdateProdutoRequest { Nome = "Produto 1", Preco = 10m };

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Produto?)null);

        var result = await _sut.Update(id, request);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Update_DeveAtualizarProduto_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var produto = new Produto { Id = id, Nome = "Produto Antigo", Preco = 10m, Ativo = true };
        var request = new UpdateProdutoRequest { Nome = "Produto Novo", Preco = 20m, Descricao = "Nova descripción", Ativo = false };
        var produtoDto = new ProdutoDto { Id = id, Nome = "Produto Novo", Preco = 20m, Descricao = "Nueva descripción", Ativo = false };

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(produto);
        _mapperMock
            .Setup(x => x.Map<ProdutoDto>(produto))
            .Returns(produtoDto);
        _produtoRepositoryMock
            .Setup(x => x.Update(It.IsAny<Produto>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Update(id, request);

        result.IsSuccess.Should().BeTrue();
        _produtoRepositoryMock.Verify(x => x.Update(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_DeveRetornarErro_QuandoProdutoNaoExistir()
    {
        var id = Guid.NewGuid();

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Produto?)null);

        var result = await _sut.Delete(id);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task Delete_DeveExcluirProduto_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var produto = new Produto { Id = id, Nome = "Produto 1", Preco = 10m, Ativo = true };

        _produtoRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(produto);
        _produtoRepositoryMock
            .Setup(x => x.Delete(It.IsAny<Produto>()));
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var result = await _sut.Delete(id);

        result.IsSuccess.Should().BeTrue();
        _produtoRepositoryMock.Verify(x => x.Delete(produto), Times.Once);
    }
}
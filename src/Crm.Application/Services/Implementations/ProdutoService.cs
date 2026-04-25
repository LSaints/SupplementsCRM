using AutoMapper;
using Crm.Application.DTOs.Produto;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using FluentResults;

namespace Crm.Application.Services.Implementations;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProdutoService(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<ProdutoDto>>> GetAll(bool incluirInativos = false)
    {
        var produtos = await _produtoRepository.GetAllAsync(incluirInativos);
        return Result.Ok(_mapper.Map<List<ProdutoDto>>(produtos));
    }

    public async Task<Result<ProdutoDto>> GetById(Guid id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        
        return produto is not null
            ? Result.Ok(_mapper.Map<ProdutoDto>(produto))
            : Result.Fail(new Error($"Produto {id} não encontrado."));
    }

    public async Task<Result<ProdutoDto>> Create(CreateProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return Result.Fail(new Error("Nome é obrigatório."));

        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Preco = request.Preco,
            Descricao = request.Descricao,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _produtoRepository.Add(produto);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<ProdutoDto>(produto));
    }

    public async Task<Result<ProdutoDto>> Update(Guid id, UpdateProdutoRequest request)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto is null)
            return Result.Fail(new Error($"Produto {id} não encontrado."));

        produto.Nome = request.Nome;
        produto.Preco = request.Preco;
        produto.Descricao = request.Descricao;
        produto.Ativo = request.Ativo;
        produto.CriadoEm = DateTime.UtcNow;

        _produtoRepository.Update(produto);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<ProdutoDto>(produto));
    }

    public async Task<Result> Delete(Guid id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto is null)
            return Result.Fail(new Error($"Produto {id} não encontrado."));

        _produtoRepository.Delete(produto);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
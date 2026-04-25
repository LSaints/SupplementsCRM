using Crm.Application.DTOs.Produto;
using FluentResults;

namespace Crm.Application.Services.Interfaces;

public interface IProdutoService
{
    Task<Result<List<ProdutoDto>>> GetAll(bool incluirInativos = false);
    Task<Result<ProdutoDto>> GetById(Guid id);
    Task<Result<ProdutoDto>> Create(CreateProdutoRequest request);
    Task<Result<ProdutoDto>> Update(Guid id, UpdateProdutoRequest request);
    Task<Result> Delete(Guid id);
}
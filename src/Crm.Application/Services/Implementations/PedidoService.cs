using AutoMapper;
using Crm.Application.DTOs.Pedido;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentResults;

namespace Crm.Application.Services.Implementations;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<PedidoDto>>> GetAll()
    {
        var pedidos = await _pedidoRepository.GetAllAsync();
        return Result.Ok(_mapper.Map<List<PedidoDto>>(pedidos));
    }

    public async Task<Result<List<PedidoDto>>> GetByVendedorId(Guid vendedorId)
    {
        var pedidos = await _pedidoRepository.GetByVendedorIdAsync(vendedorId);
        return Result.Ok(_mapper.Map<List<PedidoDto>>(pedidos));
    }

    public async Task<Result<List<PedidoDto>>> GetByClienteId(Guid clienteId)
    {
        var pedidos = await _pedidoRepository.GetByClienteIdAsync(clienteId);
        return Result.Ok(_mapper.Map<List<PedidoDto>>(pedidos));
    }

    public async Task<Result<PedidoDto>> GetById(Guid id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        
        return pedido is not null
            ? Result.Ok(_mapper.Map<PedidoDto>(pedido))
            : Result.Fail(new Error($"Pedido {id} não encontrado."));
    }

    public async Task<Result<PedidoDto>> Create(CreatePedidoRequest request)
    {
        if (request.Itens is null || !request.Itens.Any())
            return Result.Fail(new Error("Pelo menos um item é obrigatório."));

        var valorTotal = 0m;
        var itens = new List<PedidoItem>();

        foreach (var itemRequest in request.Itens)
        {
            var produto = await _produtoRepository.GetByIdAsync(itemRequest.ProdutoId);
            if (produto is null)
                return Result.Fail(new Error($"Produto {itemRequest.ProdutoId} não encontrado."));

            var precoUnitario = produto.Preco;
            valorTotal += precoUnitario * itemRequest.Quantidade;

            itens.Add(new PedidoItem
            {
                Id = Guid.NewGuid(),
                ProdutoId = produto.Id,
                Produto = produto,
                Quantidade = itemRequest.Quantidade,
                PrecoUnitario = precoUnitario
            });
        }

        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteId = request.ClienteId,
            VendedorId = request.VendedorId,
            Itens = itens,
            ValorTotal = valorTotal,
            Status = StatusPedido.Pendente,
            CriadoEm = DateTime.UtcNow
        };

        _pedidoRepository.Add(pedido);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<PedidoDto>(pedido));
    }

    public async Task<Result<PedidoDto>> Update(Guid id, CreatePedidoRequest request)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        if (pedido is null)
            return Result.Fail(new Error($"Pedido {id} não encontrado."));

        if (request.Itens is null || !request.Itens.Any())
            return Result.Fail(new Error("Pelo menos um item é obrigatório."));

        foreach (var item in pedido.Itens)
        {
            _pedidoRepository.RemoveItem(item);
        }
        
        pedido.Itens.Clear();
        
        var valorTotal = 0m;

        foreach (var itemRequest in request.Itens)
        {
            var produto = await _produtoRepository.GetByIdAsync(itemRequest.ProdutoId);
            if (produto is null)
                return Result.Fail(new Error($"Produto {itemRequest.ProdutoId} não encontrado."));

            valorTotal += produto.Preco * itemRequest.Quantidade;

            pedido.Itens.Add(new PedidoItem
            {
                Id = Guid.NewGuid(),
                PedidoId = pedido.Id,
                ProdutoId = produto.Id,
                Produto = produto,
                Quantidade = itemRequest.Quantidade,
                PrecoUnitario = produto.Preco
            });
        }

        pedido.ClienteId = request.ClienteId;
        pedido.ValorTotal = valorTotal;
        pedido.AtualizadoEm = DateTime.UtcNow;

        _pedidoRepository.Update(pedido);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<PedidoDto>(pedido));
    }

    public async Task<Result<PedidoDto>> UpdateStatus(Guid id, StatusPedido status)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        if (pedido is null)
            return Result.Fail(new Error($"Pedido {id} não encontrado."));

        pedido.Status = status;
        pedido.AtualizadoEm = DateTime.UtcNow;

        _pedidoRepository.Update(pedido);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<PedidoDto>(pedido));
    }

    public async Task<Result> Delete(Guid id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        if (pedido is null)
            return Result.Fail(new Error($"Pedido {id} não encontrado."));

        _pedidoRepository.Delete(pedido);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
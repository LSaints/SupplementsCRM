using AutoMapper;
using Crm.Application.DTOs.Checkout;
using Crm.Application.DTOs.Pedido;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Domain.Interfaces;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace Crm.Application.Services.Implementations;

public class CheckoutService : ICheckoutService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ILinkPagamentoRepository _linkPagamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _checkoutUrlBase;

    public CheckoutService(
        IPedidoRepository pedidoRepository,
        ILinkPagamentoRepository linkPagamentoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration)
    {
        _pedidoRepository = pedidoRepository;
        _linkPagamentoRepository = linkPagamentoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _checkoutUrlBase = configuration["Checkout:UrlBase"] ?? "http://localhost:5173/checkout";
    }

    public async Task<Result<LinkPagamentoDto>> CriarLinkPagamento(CreateLinkPagamentoRequest request)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(request.PedidoId);
        if (pedido is null)
            return Result.Fail(new Error($"Pedido {request.PedidoId} não encontrado."));

        var linkPagamentoExistente = await _linkPagamentoRepository.GetByPedidoIdAsync(request.PedidoId);
        
        if (linkPagamentoExistente is not null)
        {
            linkPagamentoExistente.Url = $"{_checkoutUrlBase}/{pedido.Id}";
            linkPagamentoExistente.ExpiraEm = DateTime.UtcNow.AddDays(request.DiasValidade);
            linkPagamentoExistente.Utilizado = false;
            _linkPagamentoRepository.Update(linkPagamentoExistente);
            pedido.LinkPagamento = linkPagamentoExistente.Url;
            _pedidoRepository.Update(pedido);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(_mapper.Map<LinkPagamentoDto>(linkPagamentoExistente));
        }

        var linkPagamento = new LinkPagamento
        {
            Id = Guid.NewGuid(),
            PedidoId = pedido.Id,
            Url = $"{_checkoutUrlBase}/{pedido.Id}",
            CriadoEm = DateTime.UtcNow,
            ExpiraEm = DateTime.UtcNow.AddDays(request.DiasValidade),
            Utilizado = false
        };

        _linkPagamentoRepository.Add(linkPagamento);
        
        pedido.LinkPagamento = linkPagamento.Url;
        _pedidoRepository.Update(pedido);
        
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<LinkPagamentoDto>(linkPagamento));
    }

    public async Task<Result<LinkPagamentoDto>> GetLinkPagamento(Guid pedidoId)
    {
        var linkPagamento = await _linkPagamentoRepository.GetByPedidoIdAsync(pedidoId);
        
        return linkPagamento is not null
            ? Result.Ok(_mapper.Map<LinkPagamentoDto>(linkPagamento))
            : Result.Fail(new Error($"Link de pagamento para pedido {pedidoId} não encontrado."));
    }

    public async Task<Result> MarcarUtilizado(Guid pedidoId)
    {
        var linkPagamento = await _linkPagamentoRepository.GetByPedidoIdAsync(pedidoId);
        if (linkPagamento is null)
            return Result.Fail(new Error($"Link de pagamento para pedido {pedidoId} não encontrado."));

        var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
        if (pedido is null)
            return Result.Fail(new Error($"Pedido {pedidoId} não encontrado."));

        linkPagamento.Utilizado = true;
        _linkPagamentoRepository.Update(linkPagamento);

        pedido.Status = StatusPedido.Pago;
        _pedidoRepository.Update(pedido);

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<PedidoDto>> GetPedidoParaCheckout(Guid pedidoId)
    {
        var linkPagamento = await _linkPagamentoRepository.GetByPedidoIdAsync(pedidoId);
        if (linkPagamento is null)
            return Result.Fail(new Error($"Link de pagamento para pedido {pedidoId} não encontrado."));

        if (linkPagamento.Utilizado)
            return Result.Fail(new Error("Este link de pagamento já foi utilizado."));

        var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
        if (pedido is null)
            return Result.Fail(new Error($"Pedido {pedidoId} não encontrado."));

        return Result.Ok(_mapper.Map<PedidoDto>(pedido));
    }
}
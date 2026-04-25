using Crm.Application.DTOs.Checkout;
using Crm.Application.DTOs.Pedido;
using Crm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _service;

    public CheckoutController(ICheckoutService service)
    {
        _service = service;
    }

    [HttpPost("link")]
    [Authorize]
    public async Task<IActionResult> CriarLinkPagamento([FromBody] CreateLinkPagamentoRequest request)
    {
        var result = await _service.CriarLinkPagamento(request);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpGet("link/{pedidoId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLinkPagamento(Guid pedidoId)
    {
        var result = await _service.GetLinkPagamento(pedidoId);
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpGet("{pedidoId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCheckoutData(Guid pedidoId)
    {
        var result = await _service.GetPedidoParaCheckout(pedidoId);
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpPost("link/{pedidoId:guid}/confirmar")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmarPagamento(Guid pedidoId)
    {
        var result = await _service.MarcarUtilizado(pedidoId);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Ok();
    }
}
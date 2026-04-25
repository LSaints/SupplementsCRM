using Crm.Application.DTOs.Pedido;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;

    public PedidosController(IPedidoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _service.GetAll();
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpGet("vendedor/{vendedorId:guid}")]
    public async Task<IActionResult> GetByVendedorId(Guid vendedorId)
    {
        var result = await _service.GetByVendedorId(vendedorId);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpGet("cliente/{clienteId:guid}")]
    public async Task<IActionResult> GetByClienteId(Guid clienteId)
    {
        var result = await _service.GetByClienteId(clienteId);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetById(id);
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePedidoRequest request)
    {
        var result = await _service.Create(request);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Created($"/pedidos/{result.Value.Id}", result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePedidoRequest request)
    {
        var result = await _service.Update(id, request);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusPedido status)
    {
        var result = await _service.UpdateStatus(id, status);
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.Delete(id);
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));
        return NoContent();
    }
}
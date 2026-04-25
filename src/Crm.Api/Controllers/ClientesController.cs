using Crm.Application.DTOs.Cliente;
using Crm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;

    public ClientesController(IClienteService service)
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetById(id);
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest request)
    {
        var result = await _service.Create(request);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Created($"/api/clientes/{result.Value.Id}", result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClienteRequest request)
    {
        var result = await _service.Update(id, request);
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
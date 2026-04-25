using Crm.Application.DTOs.Produto;
using Crm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutosController(IProdutoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool incluirInativos = false)
    {
        var result = await _service.GetAll(incluirInativos);
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
    public async Task<IActionResult> Create([FromBody] CreateProdutoRequest request)
    {
        var result = await _service.Create(request);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Created($"/produtos/{result.Value.Id}", result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProdutoRequest request)
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
using Crm.Application.DTOs.Webhook;
using Crm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookService _service;

    public WebhookController(IWebhookService service)
    {
        _service = service;
    }

    [HttpPost("pagamento")]
    public async Task<IActionResult> ProcessarPagamento([FromBody] WebhookRequest request)
    {
        var result = await _service.ProcessarPagamentoWebhook(request);
        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));
        return Ok();
    }
}
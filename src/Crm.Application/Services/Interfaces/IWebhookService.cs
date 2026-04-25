using Crm.Application.DTOs.Webhook;
using FluentResults;

namespace Crm.Application.Services.Interfaces;

public interface IWebhookService
{
    Task<Result> ProcessarPagamentoWebhook(WebhookRequest request);
}
namespace Crm.Application.DTOs.Webhook;

public class WebhookRequest
{
    public string? Event { get; set; }
    public string? PaymentId { get; set; }
    public string? Status { get; set; }
}
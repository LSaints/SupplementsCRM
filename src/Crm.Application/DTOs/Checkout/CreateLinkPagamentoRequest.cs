namespace Crm.Application.DTOs.Checkout;

public class CreateLinkPagamentoRequest
{
    public Guid PedidoId { get; set; }
    public int DiasValidade { get; set; } = 3;
}
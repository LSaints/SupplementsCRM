using Crm.Domain.Entities;

namespace Crm.Domain.Entities;

public class LinkPagamento
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Pedido? Pedido { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? PaymentId { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? ExpiraEm { get; set; }
    public bool Utilizado { get; set; }
}
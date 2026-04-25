namespace Crm.Application.DTOs.Checkout;

public class LinkPagamentoDto
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime? ExpiraEm { get; set; }
    public bool Utilizado { get; set; }
}
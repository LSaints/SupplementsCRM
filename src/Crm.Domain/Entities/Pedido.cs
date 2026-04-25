using Crm.Domain.Enums;

namespace Crm.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public Guid? VendedorId { get; set; }
    public Usuario? Vendedor { get; set; }
    public List<PedidoItem> Itens { get; set; } = new();
    public decimal ValorTotal { get; set; }
    public StatusPedido Status { get; set; }
    public string? LinkPagamento { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
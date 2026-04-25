using Crm.Domain.Enums;

namespace Crm.Application.DTOs.Pedido;

public class PedidoDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public Guid? VendedorId { get; set; }
    public string? VendedorNome { get; set; }
    public List<PedidoItemDto> Itens { get; set; } = new();
    public decimal ValorTotal { get; set; }
    public StatusPedido Status { get; set; }
    public string? LinkPagamento { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
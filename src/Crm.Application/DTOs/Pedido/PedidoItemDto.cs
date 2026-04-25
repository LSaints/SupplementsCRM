namespace Crm.Application.DTOs.Pedido;

public class PedidoItemDto
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
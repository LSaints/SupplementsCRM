using Crm.Domain.Enums;

namespace Crm.Application.DTOs.Pedido;

public class CreatePedidoRequest
{
    public Guid ClienteId { get; set; }
    public Guid? VendedorId { get; set; }
    public List<CreatePedidoItemRequest> Itens { get; set; } = new();
}

public class CreatePedidoItemRequest
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
}
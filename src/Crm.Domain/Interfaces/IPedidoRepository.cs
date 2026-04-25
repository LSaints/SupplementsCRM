using Crm.Domain.Entities;

public interface IPedidoRepository
{
    Task<Pedido?> GetByIdAsync(Guid id);
    Task<List<Pedido>> GetAllAsync();
    Task<List<Pedido>> GetByVendedorIdAsync(Guid vendedorId);
    Task<List<Pedido>> GetByClienteIdAsync(Guid clienteId);
    void Add(Pedido pedido);
    void Update(Pedido pedido);
    void Delete(Pedido pedido);
    void RemoveItens(List<PedidoItem> itens);
    void RemoveItem(PedidoItem item);
}
using Crm.Domain.Entities;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(Guid id);
    Task<List<Cliente>> GetAllAsync();
    Task<List<Cliente>> GetByVendedorIdAsync(Guid vendedorId);
    void Add(Cliente cliente);
    void Update(Cliente cliente);
    void Delete(Cliente cliente);
}
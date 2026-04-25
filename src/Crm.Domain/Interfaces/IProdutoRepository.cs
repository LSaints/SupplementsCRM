using Crm.Domain.Entities;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id);
    Task<List<Produto>> GetAllAsync(bool incluirInativos = false);
    void Add(Produto produto);
    void Update(Produto produto);
    void Delete(Produto produto);
}
using Crm.Domain.Entities;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(Guid id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<List<Usuario>> GetAllAsync();
    void Add(Usuario usuario);
    void Update(Usuario usuario);
    void Delete(Usuario usuario);
}
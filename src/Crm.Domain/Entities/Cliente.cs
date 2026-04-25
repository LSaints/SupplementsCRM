using Crm.Domain.ValueObjects;

namespace Crm.Domain.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    
    public Endereco? Endereco { get; set; }
    public Guid? VendedorId { get; set; }
    public Usuario? Vendedor { get; set; }
    public DateTime CriadoEm { get; set; }
}
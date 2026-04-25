namespace Crm.Application.DTOs.Cliente;

public class UpdateClienteRequest
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    
    public EnderecoDto? Endereco { get; set; }
    public Guid? VendedorId { get; set; }
}
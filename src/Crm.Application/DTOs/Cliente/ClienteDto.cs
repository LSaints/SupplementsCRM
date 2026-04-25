namespace Crm.Application.DTOs.Cliente;

public class ClienteDto
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    
    public EnderecoDto? Endereco { get; set; }
    public Guid? VendedorId { get; set; }
    public string? VendedorNome { get; set; }
    public DateTime CriadoEm { get; set; }
}
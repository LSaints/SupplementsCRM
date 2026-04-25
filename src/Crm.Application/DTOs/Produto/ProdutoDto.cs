namespace Crm.Application.DTOs.Produto;

public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
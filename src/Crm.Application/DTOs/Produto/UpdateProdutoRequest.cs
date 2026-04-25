namespace Crm.Application.DTOs.Produto;

public class UpdateProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
}
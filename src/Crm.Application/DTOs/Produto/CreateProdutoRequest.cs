namespace Crm.Application.DTOs.Produto;

public class CreateProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
}
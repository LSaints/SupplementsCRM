namespace Crm.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
}
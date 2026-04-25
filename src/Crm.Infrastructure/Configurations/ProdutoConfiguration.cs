using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Crm.Domain.Entities;

namespace Crm.Infrastructure.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.Preco)
            .HasPrecision(18, 2);
            
        builder.Property(x => x.Descricao)
            .HasMaxLength(1000);
    }
}
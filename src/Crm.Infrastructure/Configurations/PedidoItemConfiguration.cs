using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Crm.Domain.Entities;

namespace Crm.Infrastructure.Configurations;

public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
{
    public void Configure(EntityTypeBuilder<PedidoItem> builder)
    {
        builder.ToTable("PedidoItens");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Quantidade)
            .IsRequired();
            
        builder.Property(x => x.PrecoUnitario)
            .HasPrecision(18, 2);
            
        builder.HasOne(x => x.Pedido)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
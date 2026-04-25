using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Crm.Domain.Entities;
using Crm.Domain.Enums;

namespace Crm.Infrastructure.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ValorTotal)
            .HasPrecision(18, 2);
            
        builder.Property(x => x.Status)
            .HasConversion<string>();
            
        builder.Property(x => x.LinkPagamento)
            .HasMaxLength(500);
            
        builder.HasOne(x => x.Cliente)
            .WithMany()
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(x => x.Vendedor)
            .WithMany()
            .HasForeignKey(x => x.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(x => x.Itens)
            .WithOne(x => x.Pedido)
            .HasForeignKey(x => x.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Crm.Domain.Entities;

namespace Crm.Infrastructure.Configurations;

public class LinkPagamentoConfiguration : IEntityTypeConfiguration<LinkPagamento>
{
    public void Configure(EntityTypeBuilder<LinkPagamento> builder)
    {
        builder.ToTable("LinksPagamento");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.PaymentId)
            .HasMaxLength(100);
            
        builder.HasOne(x => x.Pedido)
            .WithMany()
            .HasForeignKey(x => x.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => x.PedidoId).IsUnique();
    }
}
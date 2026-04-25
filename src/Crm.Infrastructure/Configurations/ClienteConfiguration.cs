using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Crm.Domain.Entities;
using Crm.Domain.ValueObjects;

namespace Crm.Infrastructure.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Nome)
            .HasMaxLength(200);
            
        builder.Property(x => x.Email)
            .HasMaxLength(200);
            
        builder.Property(x => x.Telefone)
            .HasMaxLength(20);
            
        
            
        builder.OwnsOne(x => x.Endereco, endereco =>
        {
            endereco.Property(e => e.Rua)
                .HasMaxLength(200)
                .HasColumnName("Endereco_Rua");
                
            endereco.Property(e => e.Numero)
                .HasMaxLength(20)
                .HasColumnName("Endereco_Numero");
                
            endereco.Property(e => e.Complemento)
                .HasMaxLength(200)
                .HasColumnName("Endereco_Complemento");
                
            endereco.Property(e => e.Bairro)
                .HasMaxLength(100)
                .HasColumnName("Endereco_Bairro");
                
            endereco.Property(e => e.Cidade)
                .HasMaxLength(100)
                .HasColumnName("Endereco_Cidade");
                
            endereco.Property(e => e.Estado)
                .HasMaxLength(100)
                .HasColumnName("Endereco_Estado");
                
            endereco.Property(e => e.Cep)
                .HasMaxLength(10)
                .HasColumnName("Endereco_Cep");
        });
            
        builder.HasOne(x => x.Vendedor)
            .WithMany()
            .HasForeignKey(x => x.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
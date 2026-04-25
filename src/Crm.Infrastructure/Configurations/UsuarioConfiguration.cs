using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Crm.Domain.Entities;

namespace Crm.Infrastructure.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.SenhaHash)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.Role)
            .HasMaxLength(50);
            
        builder.HasIndex(x => x.Email).IsUnique();
    }
}
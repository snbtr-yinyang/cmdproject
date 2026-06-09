using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CMDProject.Domain.Entities;

namespace CMDProject.Infrastructure.Persistence.Configurations;

public class AuthUserConfiguration : IEntityTypeConfiguration<AuthUsers>
{
    public void Configure(EntityTypeBuilder<AuthUsers> builder)
    {
        builder.ToTable("MsAuthUsers");

        builder.HasKey(x => x.AuthUserId);

        builder.Property(x => x.AuthUserId)
            .HasColumnName("AuthUserId");

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(x => x.AuthUserName)
            .HasColumnName("AuthUserName")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.AuthPassword)
            .HasColumnName("AuthPassword")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .HasColumnName("CreatedDate")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasMaxLength(50)
            .IsRequired();
    }
}

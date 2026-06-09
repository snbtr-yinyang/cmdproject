using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CMDProject.Domain.Entities;

namespace CMDProject.Infrastructure.Persistence.Configurations;
public class TokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("TrUserRefreshTokens");

        builder.HasKey(x => x.IdToken);

        builder.Property(x => x.IdToken)
            .HasColumnName("IdToken");

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(x => x.TokenName)
            .HasColumnName("TokenName")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ActiveStatus)
            .HasColumnName("ActiveStatus")
            .IsRequired();

        builder.Property(x => x.ExpiredDate)
            .HasColumnName("ExpiredDate")
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .HasColumnName("CreatedDate")
            .IsRequired();

        builder.Property(x => x.RevokedDate)
            .HasColumnName("RevokedDate");
    }
}

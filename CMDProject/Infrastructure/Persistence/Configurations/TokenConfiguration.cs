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

        builder.Property(x => x.Users_Id)
            .IsRequired();

        builder.Property(x => x.TokenName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ActiveStatus)
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(x => x.Expired_Date)
            .IsRequired();

        builder.Property(x => x.Created_Date)
            .IsRequired();

        builder.Property(x => x.Revoked_Date)
            .IsRequired();
    }
}

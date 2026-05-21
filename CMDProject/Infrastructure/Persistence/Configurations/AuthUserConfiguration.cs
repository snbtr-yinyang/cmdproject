using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CMDProject.Domain.Entities;

namespace CMDProject.Infrastructure.Persistence.Configurations;

public class AuthUserConfiguration : IEntityTypeConfiguration<MsAuthUsers>
{
    public void Configure(EntityTypeBuilder<MsAuthUsers> builder)
    {
        builder.ToTable("MsAuthUsers");

        builder.HasKey(x => x.AuthUsers_Id);

        builder.Property(x => x.Users_Id)
            .IsRequired();

        builder.Property(x => x.Auth_UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Auth_Password)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Created_Date)
            .IsRequired();

        builder.Property(x => x.Created_By)
            .HasMaxLength(50)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CMDProject.Domain.Entities;

namespace CMDProject.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<MsUsers>
{
    public void Configure(EntityTypeBuilder<MsUsers> builder)
    {
        builder.ToTable("MsUsers");

        builder.HasKey(x => x.Users_Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(x => x.MobilePhone)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(x => x.Gender_Id)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Religion_Id)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Is_Active)
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(x => x.Created_Date)
            .IsRequired();

        builder.Property(x => x.Created_By)
            .HasMaxLength(50)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CMDProject.Domain.Entities;

namespace CMDProject.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.ToTable("MsUsers");

        builder.HasKey(x => x.UserId);

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

        builder.Property(x => x.GenderId)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.ReligionId)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(50)
            .IsRequired();
    }
}

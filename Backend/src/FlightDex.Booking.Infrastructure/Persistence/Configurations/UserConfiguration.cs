using FlightDex.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Booking.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Email).HasColumnType("varchar(256)").IsRequired();
        builder.Property(u => u.FirstName).HasColumnType("varchar(128)").IsRequired();
        builder.Property(u => u.LastName).HasColumnType("varchar(128)").IsRequired();
        builder.Property(u => u.Age).IsRequired();
        builder.Property(u => u.IsGovernmentOfficial).IsRequired();
        builder.Property(u => u.IsLawEnforcementOrMilitary).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnType("varchar(256)").IsRequired();
        builder.Property(u => u.PasswordSalt).HasColumnType("varchar(256)").IsRequired();

        // One account per email.
        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");
    }
}

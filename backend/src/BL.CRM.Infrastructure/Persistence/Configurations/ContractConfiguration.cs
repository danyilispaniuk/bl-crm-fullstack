using BL.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BL.CRM.Infrastructure.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);

        // Make RegistrationNumber unique
        builder.HasIndex(c => c.RegistrationNumber)
            .IsUnique();

        builder.Property(c => c.Institution)
            .IsRequired()
            .HasMaxLength(200);

        // Relationships
        builder.HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ContractManager)
            .WithMany(a => a.ManagedContracts)
            .HasForeignKey(c => c.ContractManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many for participants
        builder.HasMany(c => c.Participants)
            .WithMany(a => a.ParticipatedContracts)
            .UsingEntity(j => j.ToTable("ContractParticipants"));
    }
}

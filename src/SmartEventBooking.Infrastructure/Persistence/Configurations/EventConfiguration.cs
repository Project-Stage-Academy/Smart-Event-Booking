using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEventBooking.Domain.Entities;

namespace SmartEventBooking.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events", t => t.HasCheckConstraint("CK_Event_AvailableSeats", "AvailableSeats >= 0"));

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired(false);

        builder.Property(e => e.Banner)
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(e => e.StartDateTime)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.EndDateTime)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.TotalCapacity)
            .IsRequired();

        builder.Property(e => e.AvailableSeats)
            .IsRequired();

        builder.Property(e => e.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasColumnType("tinyint");

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.Property(e => e.VenueId)
            .IsRequired();

        builder.HasIndex(e => e.Title);
        builder.HasIndex(e => e.StartDateTime);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Price);
        builder.HasIndex(e => e.VenueId);
    }
}

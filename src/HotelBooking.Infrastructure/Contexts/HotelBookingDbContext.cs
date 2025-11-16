using BookingManagement;
using RoomManagement;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure;

public class HotelBookingDbContext : DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options)
        : base(options)
    {
    }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Room> Rooms { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.RoomId).IsRequired();
            entity.Property(e => e.HotelId).IsRequired();
            entity.Property(e => e.CheckInDate).IsRequired();
            entity.Property(e => e.CheckOutDate).IsRequired();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HotelId).IsRequired();
            entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RoomType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PricePerNight).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Capacity).IsRequired();
        });
    }
}

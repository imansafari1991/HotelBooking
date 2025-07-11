using HotelBooking.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Context;

public class HotelBookingDbContext : DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options)
        : base(options)
    {
    }
    public DbSet<Room> Rooms { get; set; } = null!;
    
}
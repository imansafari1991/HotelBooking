
using HotelBooking.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement;

public class BookingRepository : IBookingRepository
{
    private readonly HotelBookingDbContext _context;

    public BookingRepository(HotelBookingDbContext context)
    {
        _context = context;
    }

    public Task<Booking?> GetByIdAsync(int bookingId)
    {
        return _context.Bookings.FirstOrDefaultAsync(p => p.Id == bookingId);
    }

    public Task<bool> IsRoomBookedAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
    {
        
        return _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            (b.CheckInDate >= checkIn && b.CheckOutDate >= checkIn) ||
            (b.CheckInDate <= checkOut && b.CheckOutDate <= checkOut));
    }

    public async Task SaveAsync(Booking booking)
    {
        _context.Add(booking);
        await _context.SaveChangesAsync();
    }
}
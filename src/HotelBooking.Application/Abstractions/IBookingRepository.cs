using System;
using System.Collections.Generic;
using System.Text;

namespace BookingManagement;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int bookingId);
    Task<bool> IsRoomBookedAsync(int roomId,DateOnly checkIn, DateOnly checkOut);
    Task SaveAsync(Booking booking);
}

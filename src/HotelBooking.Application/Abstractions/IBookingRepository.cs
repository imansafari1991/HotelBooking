using System;
using System.Collections.Generic;
using System.Text;

namespace BookingManagement;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int bookingId);
    bool IsRoomBooked(int roomId,DateOnly checkIn, DateOnly checkOut);
    void Save(Booking booking);
}

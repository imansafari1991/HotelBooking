using System;
using System.Collections.Generic;
using System.Text;

namespace BookingManagement;

public class BookingErrorMessages
{
    public const string CheckOutBeforeCheckIn = "Check-out date must be after check-in date.";
    public const string RoomAlreadyBooked = "Room is already booked for the selected dates.";
    public const string BookingNotFound = "Booking with ID {{bookingId}} not found.";
    public const string Unauthorized = "You do not have access to this booking.";

}

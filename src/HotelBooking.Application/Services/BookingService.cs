

namespace BookingManagement;

public class BookingService(IBookingRepository bookingRepository)
{
    public Booking CreateBooking(BookingRequest bookingRequest)
    {

        if (bookingRequest.CheckOutDate <= bookingRequest.CheckInDate)
        {
            throw new ArgumentException(BookingErrorMessages.CheckOutBeforeCheckIn);
        }
        if (bookingRepository.IsRoomBooked(bookingRequest.RoomId, bookingRequest.CheckInDate, bookingRequest.CheckOutDate))
        {
            throw new InvalidOperationException(BookingErrorMessages.RoomAlreadyBooked);
        }

        var booking = new Booking(
              bookingRequest.CustomerId,
              bookingRequest.HotelId,
              bookingRequest.RoomId,
              bookingRequest.CheckInDate,
              bookingRequest.CheckOutDate);
        bookingRepository.Save(booking);
        return booking;
    }

    public async Task<Booking> GetBookingAsync(int bookingId, int customerId)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new KeyNotFoundException(string.Format(BookingErrorMessages.BookingNotFound, bookingId));
        }
        if (booking.CustomerId != customerId)
        {
            throw new UnauthorizedAccessException(BookingErrorMessages.Unauthorized);
        }
        return booking;
    }
}
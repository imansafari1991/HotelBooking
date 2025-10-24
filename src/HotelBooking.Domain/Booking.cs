namespace BookingManagement;

public class Booking
{
    public int Id { get; set; }
    public Booking(int customerId, int hotelId, int roomId, DateOnly checkInDate, DateOnly checkOutDate)
    {
        CustomerId = customerId;
        HotelId = hotelId;
        RoomId = roomId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
    }

    public int CustomerId { get; }
    public int HotelId { get; }
    public int RoomId { get; }
    public DateOnly CheckInDate { get;}
    public DateOnly CheckOutDate { get; }

}

namespace BookingManagement;

public class BookingRequest
{
    public int CustomerId { get; set; }
    public int HotelId { get; set; }
    public int RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }

}
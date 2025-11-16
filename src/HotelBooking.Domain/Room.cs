namespace RoomManagement;

public class Room
{
    public int Id { get; set; }
    
    public Room(int hotelId, string roomNumber, string roomType, decimal pricePerNight, int capacity)
    {
        HotelId = hotelId;
        RoomNumber = roomNumber;
        RoomType = roomType;
        PricePerNight = pricePerNight;
        Capacity = capacity;
    }

    public int HotelId { get; }
    public string RoomNumber { get; }
    public string RoomType { get; }
    public decimal PricePerNight { get; }
    public int Capacity { get; }
}

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
    public void UpdateDetails(string roomType, decimal pricePerNight, int capacity)
    {
        RoomType = roomType;
        PricePerNight = pricePerNight;
        Capacity = capacity;
    }

    public int HotelId { get; private set; }
    public string RoomNumber { get; private set; }
    public string RoomType { get; private set; }
    public decimal PricePerNight { get; private set; }
    public int Capacity { get; private set; }
}

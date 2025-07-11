using static HotelBooking.Domain.Constants.PriceExceptionMessages;

namespace HotelBooking.Domain;

public class Room
{
    private Room()
    {
    }
    public static Room Create(string roomNumber, string countryCode, decimal price)
    {
        ValidatePrice(price);
        ValidateRoomNumber(roomNumber);
        
        return new Room
        {
            Bookings = new List<Booking>(),
            RoomNumber = roomNumber,
            CountryCode = countryCode,
            Price = price
        };
    }
    public  int Id { get; set; }
    public string RoomNumber { get; private set; }
    public string RoomCode =>$"{CountryCode}-{RoomNumber}";
    public string CountryCode { get;private set; }

    public decimal Price { get;private set; }
    public List<Booking> Bookings { get; set; }

    public void UpdatePrice(decimal updatedPrice)
    {
        ValidatePrice(updatedPrice);
        Price = updatedPrice;
    }

    public void SetRoomNumber(string setNumber)
    {
        ValidateRoomNumber(setNumber);
        RoomNumber = setNumber;
    }
  
    private static void ValidateRoomNumber(string roomNumber)
    {
        if (string.IsNullOrEmpty(roomNumber))
        {
            throw new ArgumentNullException();
        }
    }
    private static void ValidatePrice(decimal updatedPrice)
    {
        if (updatedPrice < 0)
        {
            throw new ArgumentException(Price_Cant_Be_Negative);
        }
        else if (updatedPrice == 0)
        {
            throw new ArgumentException(Price_Cant_Be_Zero);
        }
        else if (updatedPrice > 10000)
        {
            throw new ArgumentException(Price_Cant_Be_More_Than_TenThousand);
        }
    }
}
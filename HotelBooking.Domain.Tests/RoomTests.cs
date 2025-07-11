using AwesomeAssertions;
using HotelBooking.Domain.Constants;

namespace HotelBooking.Domain.Tests;
using static HotelBooking.Domain.Constants.PriceExceptionMessages;

public class RoomTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CreateRoomWithValidData()
    {
        // Arrange
        var room = Room.Create("101", "US", 100.0m);
        // Act
        // Assert

        room.RoomNumber.Should().Be("101");
        room.CountryCode.Should().Be("US");
        room.Price.Should().Be(100.0m);
    }


    [TestCase(-100.0, Price_Cant_Be_Negative)]
    [TestCase(0.0, Price_Cant_Be_Zero)]
    [TestCase(10001.0, Price_Cant_Be_More_Than_TenThousand)]
    public void PreventRoomCreationWithInvalidPrices(decimal price, string expectedMessage)
    {
        // Arrange

        // Act
        var action = () => Room.Create("101", "US", price);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage(expectedMessage);
    }
    [TestCase(-100.0, Price_Cant_Be_Negative)]
    [TestCase(0.0, Price_Cant_Be_Zero)]
    [TestCase(10001.0, Price_Cant_Be_More_Than_TenThousand)]
    public void PreventRoomUpdatePriceWithInvalidValues(decimal updatedPrice, string expectedMessage)
    {
        // Arrange
        var room = Room.Create("101", "US", 100.0m);
        // Act
        var action = () => room.UpdatePrice(updatedPrice);
        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage(expectedMessage);
    }
    [Test]
    public void PreventRoomCreationWithNullRoomNumber()
    {
        // Arrange
        var action = () => Room.Create(null, "US", 100.0m);
        
        // Act & Assert
        action.Should().Throw<ArgumentNullException>();
    }
    [Test]
    public void PreventRoomUpdateWithNullRoomNumber()
    {
        var room = Room.Create("101", "US", 100.0m);
        // Arrange
        var action = () => room.SetRoomNumber(null);

        // Act & Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ShouldUpdateRoomPriceSuccessfully()
    {
        // Arrange
        var room = Room.Create("101", "US", 100.0m);
        
        // Act
        room.UpdatePrice(150.0m);
        
        // Assert
        room.Price.Should().Be(150.0m);
    }
    [Test]
    public void ShouldUpdateRoomNumberSuccessfully()
    {
        // Arrange
        var room = Room.Create("101", "US", 100.0m);
        
        // Act
        room.SetRoomNumber("102");
        
        // Assert
        room.RoomNumber.Should().Be("102");
    }
    [Test]
    public void RoomCodeShouldHaveCorrectFormat()
    {
        // Arrange
        var room = Room.Create("101", "US", 100.0m);
        var expectedValue = "US-101";
        // Act
        var actualValue = room.RoomCode;
       
        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void HaveEmptyBookingListOnRoomCreation() { 
    
        //Arrange 
        var room = Room.Create("101", "US", 100.0m);
        
        //Assert
        room.Bookings.Should().BeEmpty();

    }
}

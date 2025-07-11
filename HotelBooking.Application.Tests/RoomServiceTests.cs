using AwesomeAssertions;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain;
using Moq;

namespace HotelBooking.Application.Tests;

public class RoomServiceTests
{

    [Test]
    public async Task CreateNewRoom_ShouldCallInsertAsync()
    {
        // Arrange
        var roomStoreMock = new Mock<IRoomStore>();
        var roomService = new RoomService(roomStoreMock.Object);
        var roomNumber = "101";
        var countryCode = "US";
        var price = 150.00m;
        // Act
        await roomService.CreateNewRoomAsync(roomNumber, countryCode, price, default);
        // Assert
        roomStoreMock.Verify(store => store.InsertAsync(It.Is<Room>(p => p.Price == price && p.CountryCode == countryCode && p.RoomNumber == roomNumber)), Times.Once);
    }

    [Test]
    public async Task PreventCreateDuplicatedRoomNumber()
    {
        // Arrange
        var roomStoreMock = new Mock<IRoomStore>();
        roomStoreMock.Setup(store => store.IsExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var roomService = new RoomService(roomStoreMock.Object);
        var roomNumber = "101";
        var countryCode = "US";
        var price = 150.00m;

        // Act
        var action = async () => await roomService.CreateNewRoomAsync(roomNumber, countryCode, price, default);
        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Room with this number already exists");
        roomStoreMock.Verify(store => store.InsertAsync(It.IsAny<Room>()), Times.Never);
    }

}

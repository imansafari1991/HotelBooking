using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomManagement;

public class RoomServiceTests
{
    private RoomService _roomService;
    private Mock<IRoomRepository> _roomRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _roomService = new RoomService(_roomRepositoryMock.Object);
    }

    [Test]
    public async Task CreateRoom_WithValidData_ShouldCreateRoom()
    {
        // Arrange
        var roomRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };

        // Act
        var result = await _roomService.CreateRoom(roomRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.HotelId, Is.EqualTo(roomRequest.HotelId));
        Assert.That(result.RoomNumber, Is.EqualTo(roomRequest.RoomNumber));
        Assert.That(result.RoomType, Is.EqualTo(roomRequest.RoomType));
        Assert.That(result.PricePerNight, Is.EqualTo(roomRequest.PricePerNight));
        Assert.That(result.Capacity, Is.EqualTo(roomRequest.Capacity));
        _roomRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<Room>()), Times.Once);
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("   ")]
    public void CreateRoom_WithInvalidRoomNumber_ShouldThrowException(string roomNumber)
    {
        // Arrange
        var roomRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = roomNumber,
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _roomService.CreateRoom(roomRequest));
        Assert.That(ex.Message, Is.EqualTo(RoomErrorMessages.InvalidRoomNumber));
        _roomRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<Room>()), Times.Never);
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("   ")]
    public void CreateRoom_WithInvalidRoomType_ShouldThrowException(string roomType)
    {
        // Arrange
        var roomRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = roomType,
            PricePerNight = 150.00m,
            Capacity = 2
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _roomService.CreateRoom(roomRequest));
        Assert.That(ex.Message, Is.EqualTo(RoomErrorMessages.InvalidRoomType));
        _roomRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<Room>()), Times.Never);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-100)]
    public void CreateRoom_WithInvalidPricePerNight_ShouldThrowException(decimal price)
    {
        // Arrange
        var roomRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = price,
            Capacity = 2
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _roomService.CreateRoom(roomRequest));
        Assert.That(ex.Message, Is.EqualTo(RoomErrorMessages.InvalidPricePerNight));
        _roomRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<Room>()), Times.Never);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-5)]
    public void CreateRoom_WithInvalidCapacity_ShouldThrowException(int capacity)
    {
        // Arrange
        var roomRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = capacity
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _roomService.CreateRoom(roomRequest));
        Assert.That(ex.Message, Is.EqualTo(RoomErrorMessages.InvalidCapacity));
        _roomRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<Room>()), Times.Never);
    }

    [Test]
    public async Task GetRoomAsync_WithExistingRoom_ShouldReturnRoom()
    {
        // Arrange
        var roomId = 1;
        var expectedRoom = new Room(1, "101", "Deluxe", 150.00m, 2) { Id = roomId };
        _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomId)).ReturnsAsync(expectedRoom);

        // Act
        var result = await _roomService.GetRoomAsync(roomId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(roomId));
        Assert.That(result.RoomNumber, Is.EqualTo("101"));
    }

    [Test]
    public void GetRoomAsync_WithNonExistingRoom_ShouldThrowException()
    {
        // Arrange
        var roomId = 999;
        _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomId)).ReturnsAsync((Room?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _roomService.GetRoomAsync(roomId));
        Assert.That(ex.Message, Does.Contain(roomId.ToString()));
    }

    [Test]
    public async Task GetRoomsByHotelAsync_ShouldReturnRooms()
    {
        // Arrange
        var hotelId = 1;
        var expectedRooms = new List<Room>
        {
            new Room(hotelId, "101", "Deluxe", 150.00m, 2) { Id = 1 },
            new Room(hotelId, "102", "Standard", 100.00m, 2) { Id = 2 }
        };
        _roomRepositoryMock.Setup(repo => repo.GetRoomsByHotelIdAsync(hotelId)).ReturnsAsync(expectedRooms);

        // Act
        var result = await _roomService.GetRoomsByHotelAsync(hotelId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateRoom_WithValidData_ShouldUpdateRoom()
    {
        // Arrange
        var roomId = 1;
        var existingRoom = new Room(1, "101", "Deluxe", 150.00m, 2) { Id = roomId };
        var roomRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101A",
            RoomType = "Suite",
            PricePerNight = 200.00m,
            Capacity = 3
        };
        _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomId)).ReturnsAsync(existingRoom);

        // Act
        var result = await _roomService.UpdateRoom(roomId, roomRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(roomId));
        Assert.That(result.RoomNumber, Is.EqualTo(roomRequest.RoomNumber));
        Assert.That(result.RoomType, Is.EqualTo(roomRequest.RoomType));
        Assert.That(result.PricePerNight, Is.EqualTo(roomRequest.PricePerNight));
        Assert.That(result.Capacity, Is.EqualTo(roomRequest.Capacity));
        _roomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Room>()), Times.Once);
    }

    [Test]
    public void UpdateRoom_WithNonExistingRoom_ShouldThrowException()
    {
        // Arrange
        var roomId = 999;
        var roomRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };
        _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomId)).ReturnsAsync((Room?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _roomService.UpdateRoom(roomId, roomRequest));
        Assert.That(ex.Message, Does.Contain(roomId.ToString()));
        _roomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Room>()), Times.Never);
    }

    [Test]
    public async Task DeleteRoom_WithExistingRoom_ShouldDeleteRoom()
    {
        // Arrange
        var roomId = 1;
        var existingRoom = new Room(1, "101", "Deluxe", 150.00m, 2) { Id = roomId };
        _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomId)).ReturnsAsync(existingRoom);

        // Act
        await _roomService.DeleteRoom(roomId);

        // Assert
        _roomRepositoryMock.Verify(repo => repo.DeleteAsync(roomId), Times.Once);
    }

    [Test]
    public void DeleteRoom_WithNonExistingRoom_ShouldThrowException()
    {
        // Arrange
        var roomId = 999;
        _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomId)).ReturnsAsync((Room?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _roomService.DeleteRoom(roomId));
        Assert.That(ex.Message, Does.Contain(roomId.ToString()));
        _roomRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}

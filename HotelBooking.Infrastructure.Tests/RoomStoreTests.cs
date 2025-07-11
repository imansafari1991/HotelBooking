using AwesomeAssertions;
using HotelBooking.Domain;
using HotelBooking.Infrastructure.Stores;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Context;

public class RoomStoreTests
{
    private HotelBookingDbContext _context;
    private RoomStore _roomStore;
    [SetUp]
    public void Setup()
    {
        // Initialize any required resources or mocks here
        var dbContextOptions = new DbContextOptionsBuilder<HotelBookingDbContext>()
            .UseInMemoryDatabase("HotelBookingTestDb")
            .Options;
        _context = new HotelBookingDbContext(dbContextOptions);
        _context.Database.EnsureCreated();
        _roomStore = new RoomStore(_context);

    }
    [Test]
    public async Task SaveRoomInDataBase()
    {
        // Arrange
        var room = Room.Create("101", "US", 150.00m);
        // Act
     
        await _roomStore.InsertAsync(room);
        // Assert
        _context.Rooms.FirstOrDefault(r => r.RoomNumber == room.RoomNumber).Should().NotBeNull();
    }

    [Test]
    public async Task GetRoomByIdFromDataBase()
    {
        // Arrange
        var room = Room.Create("102", "US", 200.00m);
       
        await _roomStore.InsertAsync(room);
        // Act
        var retrievedRoom =await _roomStore.GetByIdAsync(room.Id, default);
        // Assert
        retrievedRoom.Should().NotBeNull();
        retrievedRoom.RoomNumber.Should().Be(room.RoomNumber);
        retrievedRoom.CountryCode.Should().Be(room.CountryCode);
        retrievedRoom.Price.Should().Be(room.Price);
    }
    [Test]
    public async Task ThrowNotFoundExceptionForNotExistRoom()
    {
        // Act
        var act =async ()=> await _roomStore.GetByIdAsync(999, default);
        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Room Not Found");
    }

    [Test]
    public async Task UpdateRoomWithNewValues()
    {
        // Arrange
        var room = Room.Create("103", "US", 250.00m);
        await _roomStore.InsertAsync(room);
        
        // Act
        room.SetRoomNumber("104");
        room.UpdatePrice(300.00m);
        await _roomStore.UpdateAsync(room); // Assuming InsertAsync can also update if the entity exists
        
        // Assert
        var updatedRoom = await _roomStore.GetByIdAsync(room.Id, default);
        updatedRoom.Should().NotBeNull();
        updatedRoom.RoomNumber.Should().Be("104");
        updatedRoom.CountryCode.Should().Be("US");
        updatedRoom.RoomCode.Should().Be("US-104");
        updatedRoom.Price.Should().Be(300.00m);
    }
    [Test]
    public async Task CheckExistingRoomByRoomNumber()
    {
        // Arrange
        var room = Room.Create("105", "US", 350.00m);
        await _roomStore.InsertAsync(room);
        
        // Act
        var existingRoom = await _roomStore.IsExist(room.RoomNumber, default);
        var notExistingRoom = await _roomStore.IsExist("999", default);
        // Assert
        existingRoom.Should().BeTrue();
        notExistingRoom.Should().BeFalse();
    }
    [TearDown]
    public void TearDown()
    {
        // Clean up resources after each test
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

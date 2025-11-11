using HotelBooking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomManagement;

public class RoomRepositoryTests
{
    private RoomRepository _roomRepository;
    private HotelBookingDbContext _context;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
            .UseInMemoryDatabase(databaseName: $"HotelBookingTestDb_{Guid.NewGuid()}")
            .Options;
        _context = new HotelBookingDbContext(options);
        _roomRepository = new RoomRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Rooms.RemoveRange(_context.Rooms);
        _context.SaveChanges();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task Save_Room_ShouldSaveToDatabase()
    {
        // Arrange
        var room = new Room(
            hotelId: 1,
            roomNumber: "101",
            roomType: "Deluxe",
            pricePerNight: 150.00m,
            capacity: 2
        );

        // Act
        await _roomRepository.SaveAsync(room);

        // Assert
        var savedRoom = await _roomRepository.GetByIdAsync(room.Id);
        Assert.That(savedRoom, Is.Not.Null);
        Assert.That(savedRoom.HotelId, Is.EqualTo(room.HotelId));
        Assert.That(savedRoom.RoomNumber, Is.EqualTo(room.RoomNumber));
        Assert.That(savedRoom.RoomType, Is.EqualTo(room.RoomType));
        Assert.That(savedRoom.PricePerNight, Is.EqualTo(room.PricePerNight));
        Assert.That(savedRoom.Capacity, Is.EqualTo(room.Capacity));
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistingRoom_ShouldReturnNull()
    {
        // Arrange
        var nonExistingRoomId = 999;

        // Act
        var room = await _roomRepository.GetByIdAsync(nonExistingRoomId);

        // Assert
        Assert.That(room, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_WithExistingRoom_ShouldReturnRoom()
    {
        // Arrange
        var room = new Room(
            hotelId: 1,
            roomNumber: "101",
            roomType: "Deluxe",
            pricePerNight: 150.00m,
            capacity: 2
        );
        await _roomRepository.SaveAsync(room);

        // Act
        var retrievedRoom = await _roomRepository.GetByIdAsync(room.Id);

        // Assert
        Assert.That(retrievedRoom, Is.Not.Null);
        Assert.That(retrievedRoom.Id, Is.EqualTo(room.Id));
        Assert.That(retrievedRoom.HotelId, Is.EqualTo(room.HotelId));
        Assert.That(retrievedRoom.RoomNumber, Is.EqualTo(room.RoomNumber));
        Assert.That(retrievedRoom.RoomType, Is.EqualTo(room.RoomType));
        Assert.That(retrievedRoom.PricePerNight, Is.EqualTo(room.PricePerNight));
        Assert.That(retrievedRoom.Capacity, Is.EqualTo(room.Capacity));
    }

    [Test]
    public async Task GetRoomsByHotelIdAsync_WithExistingRooms_ShouldReturnRooms()
    {
        // Arrange
        var hotelId = 1;
        var room1 = new Room(hotelId, "101", "Deluxe", 150.00m, 2);
        var room2 = new Room(hotelId, "102", "Standard", 100.00m, 2);
        var room3 = new Room(2, "201", "Suite", 300.00m, 4);

        await _roomRepository.SaveAsync(room1);
        await _roomRepository.SaveAsync(room2);
        await _roomRepository.SaveAsync(room3);

        // Act
        var rooms = await _roomRepository.GetRoomsByHotelIdAsync(hotelId);

        // Assert
        Assert.That(rooms, Is.Not.Null);
        Assert.That(rooms.Count(), Is.EqualTo(2));
        Assert.That(rooms.All(r => r.HotelId == hotelId), Is.True);
    }

    [Test]
    public async Task GetRoomsByHotelIdAsync_WithNoRooms_ShouldReturnEmptyList()
    {
        // Arrange
        var hotelId = 999;

        // Act
        var rooms = await _roomRepository.GetRoomsByHotelIdAsync(hotelId);

        // Assert
        Assert.That(rooms, Is.Not.Null);
        Assert.That(rooms.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateRoom()
    {
        // Arrange
        var room = new Room(
            hotelId: 1,
            roomNumber: "101",
            roomType: "Deluxe",
            pricePerNight: 150.00m,
            capacity: 2
        );
        await _roomRepository.SaveAsync(room);

        var updatedRoom = new Room(
            hotelId: 1,
            roomNumber: "101A",
            roomType: "Suite",
            pricePerNight: 200.00m,
            capacity: 3
        )
        {
            Id = room.Id
        };

        // Act
        await _roomRepository.UpdateAsync(updatedRoom);

        // Assert
        var retrievedRoom = await _roomRepository.GetByIdAsync(room.Id);
        Assert.That(retrievedRoom, Is.Not.Null);
        Assert.That(retrievedRoom.RoomNumber, Is.EqualTo("101A"));
        Assert.That(retrievedRoom.RoomType, Is.EqualTo("Suite"));
        Assert.That(retrievedRoom.PricePerNight, Is.EqualTo(200.00m));
        Assert.That(retrievedRoom.Capacity, Is.EqualTo(3));
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteRoom()
    {
        // Arrange
        var room = new Room(
            hotelId: 1,
            roomNumber: "101",
            roomType: "Deluxe",
            pricePerNight: 150.00m,
            capacity: 2
        );
        await _roomRepository.SaveAsync(room);

        // Act
        await _roomRepository.DeleteAsync(room.Id);

        // Assert
        var deletedRoom = await _roomRepository.GetByIdAsync(room.Id);
        Assert.That(deletedRoom, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistingRoom_ShouldNotThrowException()
    {
        // Arrange
        var nonExistingRoomId = 999;

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _roomRepository.DeleteAsync(nonExistingRoomId));
    }
}

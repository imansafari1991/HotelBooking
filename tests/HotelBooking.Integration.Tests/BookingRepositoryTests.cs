using HotelBooking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingManagement;

public class BookingRepositoryTests
{
    private BookingRepository _bookingRepository;
    private HotelBookingDbContext _context;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
        .UseInMemoryDatabase(databaseName: $"HotelBookingTestDb_{Guid.NewGuid()}")
        .Options;
        _context = new HotelBookingDbContext(options);
        _bookingRepository = new BookingRepository(_context);
    }


    [TearDown]
    public void TearDown()
    {
        _context.Bookings.RemoveRange(_context.Bookings);
        _context.SaveChanges();
    }
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [Test]
    public async Task Save_Booking_ShouldSaveToDatabase()
    {
        // Arrange
        var booking = new Booking(
            customerId: 1,
            roomId: 101,
            hotelId: 1,
            checkInDate: DateOnly.FromDateTime(DateTime.Today),
            checkOutDate: DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        );

        // Act

        await _bookingRepository.SaveAsync(booking);

        // Assert
        var savedBooking = await _bookingRepository.GetByIdAsync(booking.Id);
        Assert.That(savedBooking, Is.Not.Null);
        Assert.That(savedBooking.CustomerId, Is.EqualTo(booking.CustomerId));
        Assert.That(savedBooking.HotelId, Is.EqualTo(booking.HotelId));
        Assert.That(savedBooking.RoomId, Is.EqualTo(booking.RoomId));
        Assert.That(savedBooking.CheckInDate, Is.EqualTo(booking.CheckInDate));
        Assert.That(savedBooking.CheckOutDate, Is.EqualTo(booking.CheckOutDate));

    }
    [Test]
    public async Task GetByIdAsync_WithNonExistingBooking_ShouldReturnNull()
    {
        // Arrange
        var nonExistingBookingId = 999;
        // Act
        var booking = await _bookingRepository.GetByIdAsync(nonExistingBookingId);
        // Assert
        Assert.That(booking, Is.Null);
    }
    [Test]
    public async Task GetByIdAsync_WithExistingBooking_ShouldReturnBooking()
    {
        // Arrange
        var booking = new Booking(
            customerId: 1,
            roomId: 101,
            hotelId: 1,
            checkInDate: DateOnly.FromDateTime(DateTime.Today),
            checkOutDate: DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        );

        await _bookingRepository.SaveAsync(booking);
        // Act
        var retrievedBooking = await _bookingRepository.GetByIdAsync(booking.Id);

        // Assert
        Assert.That(retrievedBooking, Is.Not.Null);
        Assert.That(retrievedBooking.Id, Is.EqualTo(booking.Id));
        Assert.That(retrievedBooking.CustomerId, Is.EqualTo(booking.CustomerId));
        Assert.That(retrievedBooking.HotelId, Is.EqualTo(booking.HotelId));
        Assert.That(retrievedBooking.RoomId, Is.EqualTo(booking.RoomId));
        Assert.That(retrievedBooking.CheckInDate, Is.EqualTo(booking.CheckInDate));
        Assert.That(retrievedBooking.CheckOutDate, Is.EqualTo(booking.CheckOutDate));
    }

    [Test]
    public async Task IsRoomBooked_WhenRoomIsNotBooked_ShouldReturnFalse()
    {
        // Arrange
        var roomId = 101;
        var checkIn = DateOnly.FromDateTime(DateTime.Today);
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        // Act
        var isBooked =await _bookingRepository.IsRoomBookedAsync(roomId, checkIn, checkOut);
        // Assert
        Assert.That(isBooked, Is.False);
    }

    [Test]
    public async Task IsRoomBooked_WhenRoomIsBookedForSameDates_ShouldReturnTrue()
    {
        // Arrange
        var roomId = 101;
        var checkIn = DateOnly.FromDateTime(DateTime.Today);
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var booking = new Booking(
            customerId: 1,
            roomId: roomId,
            hotelId: 1,
            checkInDate: checkIn,
            checkOutDate: checkOut
        );
        await _bookingRepository.SaveAsync(booking);
        // Act
        var isBooked =await _bookingRepository.IsRoomBookedAsync(roomId, checkIn, checkOut);
        // Assert
        Assert.That(isBooked, Is.True);
    }
}


using HotelBooking.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;

namespace BookingManagement;

public class BookingAPITests
{
    private HttpClient _client;
    private WebApplicationFactory<Program> _factory;
    private DbConnection _connection;

    [SetUp]
    public void Setup()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            // Additional configuration can be done here if needed
            builder.ConfigureServices(services =>
            {
                // You can replace services here for testing purposes
                services.AddDbContext<HotelBookingDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
                services.AddScoped<IBookingRepository, BookingRepository>();
                services.AddScoped<BookingService>();
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();
                db.Database.EnsureCreated();

            });
        });
        _client = _factory.CreateClient();
    }
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
        _connection.Close();
        _connection?.Dispose();
    }
    [Test]
    public async Task CreateBooking_WithValidRequest_ReturnsCreatedResult()
    {
        
        // Arrange
        var request = new BookingRequest
        {
            CustomerId = 1,
            RoomId = 101,
            HotelId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3))
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/booking", request);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var booking = await response.Content.ReadFromJsonAsync<Booking>();
        var locationHeader = response.Headers.Location;
        Assert.That(locationHeader, Is.Not.Null);
        Assert.That(locationHeader!.ToString().ToLower(), Does.EndWith($"/api/booking/{booking!.Id}"));
        Assert.That(booking, Is.Not.Null);
        Assert.That(booking.CustomerId, Is.EqualTo(request.CustomerId));
        Assert.That(booking.RoomId, Is.EqualTo(request.RoomId));
        Assert.That(booking.HotelId, Is.EqualTo(request.HotelId));
        Assert.That(booking.CheckInDate, Is.EqualTo(request.CheckInDate));
        Assert.That(booking.CheckOutDate, Is.EqualTo(request.CheckOutDate));
        Assert.That(booking.Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task CreateBooking_WithInvalidDates_ReturnsBadRequest()
    {
        // Arrange
        var request = new BookingRequest
        {
            CustomerId = 1,
            RoomId = 102,
            HotelId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) // CheckOut before CheckIn
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/booking", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseMessage = await response.Content.ReadAsStringAsync();
        Assert.That(responseMessage, Is.EqualTo(BookingErrorMessages.CheckOutBeforeCheckIn));

    }
    [Test]
    public async Task CreateBooking_WhenRoomAlreadyBooked_ReturnsConflict()
    {
        // Arrange
        var firstRequest = new BookingRequest
        {
            CustomerId = 1,
            RoomId = 103,
            HotelId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3))
        };

        var conflictingRequest = new BookingRequest
        {
            CustomerId = 2,
            RoomId = 103, // Same room
            HotelId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)), // Overlapping dates
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4))
        };

        // Act
        await _client.PostAsJsonAsync("/api/booking", firstRequest);
        var response = await _client.PostAsJsonAsync("/api/booking", conflictingRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        var responseMessage = await response.Content.ReadAsStringAsync();
        Assert.That(responseMessage, Is.EqualTo(BookingErrorMessages.RoomAlreadyBooked));
    }
    [Test]
    public async Task GetBooking_WithValidIdAndCustomerId_ReturnsBooking()
    {
        // Arrange - Create a booking first
        var request = new BookingRequest
        {
            CustomerId = 1,
            RoomId = 104,
            HotelId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3))
        };
        var createResponse = await _client.PostAsJsonAsync("/api/booking", request);
        var createdBooking = await createResponse.Content.ReadFromJsonAsync<Booking>();

        // Act
        var response = await _client.GetAsync($"/api/booking/{createdBooking!.Id}?customerId={request.CustomerId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var booking = await response.Content.ReadFromJsonAsync<Booking>();
        Assert.That(booking, Is.Not.Null);
        Assert.That(booking.Id, Is.EqualTo(createdBooking.Id));
        Assert.That(booking.CustomerId, Is.EqualTo(request.CustomerId));
        Assert.That(booking.RoomId, Is.EqualTo(request.RoomId));
    }
    [Test]
    public async Task GetBooking_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        int nonExistentBookingId = 9999;
        int customerId = 1;

        // Act
        var response = await _client.GetAsync($"/api/booking/{nonExistentBookingId}?customerId={customerId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task GetBooking_WithWrongCustomerId_ReturnsForbidden()
    {
        // Arrange - Create a booking first
        var request = new BookingRequest
        {
            CustomerId = 1,
            RoomId = 105,
            HotelId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3))
        };
        var createResponse = await _client.PostAsJsonAsync("/api/booking", request);
        var createdBooking = await createResponse.Content.ReadFromJsonAsync<Booking>();

        // Act - Try to access with wrong customer ID
        var response = await _client.GetAsync($"/api/booking/{createdBooking!.Id}?customerId=999");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

}

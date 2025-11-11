using HotelBooking.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Net;
using System.Net.Http.Json;

namespace RoomManagement;

public class RoomAPITests
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
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<HotelBookingDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
                services.AddScoped<IRoomRepository, RoomRepository>();
                services.AddScoped<RoomService>();
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
    public async Task CreateRoom_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/room", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var room = await response.Content.ReadFromJsonAsync<Room>();
        var locationHeader = response.Headers.Location;
        Assert.That(locationHeader, Is.Not.Null);
        Assert.That(locationHeader!.ToString().ToLower(), Does.EndWith($"/api/room/{room!.Id}"));
        Assert.That(room, Is.Not.Null);
        Assert.That(room.HotelId, Is.EqualTo(request.HotelId));
        Assert.That(room.RoomNumber, Is.EqualTo(request.RoomNumber));
        Assert.That(room.RoomType, Is.EqualTo(request.RoomType));
        Assert.That(room.PricePerNight, Is.EqualTo(request.PricePerNight));
        Assert.That(room.Capacity, Is.EqualTo(request.Capacity));
    }

    [Test]
    public async Task CreateRoom_WithInvalidRoomNumber_ReturnsBadRequest()
    {
        // Arrange
        var request = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/room", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CreateRoom_WithInvalidPrice_ReturnsBadRequest()
    {
        // Arrange
        var request = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 0,
            Capacity = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/room", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetRoom_WithExistingRoom_ReturnsRoom()
    {
        // Arrange
        var createRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };
        var createResponse = await _client.PostAsJsonAsync("/api/room", createRequest);
        var createdRoom = await createResponse.Content.ReadFromJsonAsync<Room>();

        // Act
        var response = await _client.GetAsync($"/api/room/{createdRoom!.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var room = await response.Content.ReadFromJsonAsync<Room>();
        Assert.That(room, Is.Not.Null);
        Assert.That(room!.Id, Is.EqualTo(createdRoom.Id));
        Assert.That(room.RoomNumber, Is.EqualTo(createRequest.RoomNumber));
    }

    [Test]
    public async Task GetRoom_WithNonExistingRoom_ReturnsNotFound()
    {
        // Arrange
        var nonExistingRoomId = 999;

        // Act
        var response = await _client.GetAsync($"/api/room/{nonExistingRoomId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetRoomsByHotel_WithExistingRooms_ReturnsRooms()
    {
        // Arrange
        var hotelId = 1;
        var request1 = new RoomRequest { HotelId = hotelId, RoomNumber = "101", RoomType = "Deluxe", PricePerNight = 150.00m, Capacity = 2 };
        var request2 = new RoomRequest { HotelId = hotelId, RoomNumber = "102", RoomType = "Standard", PricePerNight = 100.00m, Capacity = 2 };
        await _client.PostAsJsonAsync("/api/room", request1);
        await _client.PostAsJsonAsync("/api/room", request2);

        // Act
        var response = await _client.GetAsync($"/api/room/hotel/{hotelId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var rooms = await response.Content.ReadFromJsonAsync<IEnumerable<Room>>();
        Assert.That(rooms, Is.Not.Null);
        Assert.That(rooms!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateRoom_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var createRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };
        var createResponse = await _client.PostAsJsonAsync("/api/room", createRequest);
        var createdRoom = await createResponse.Content.ReadFromJsonAsync<Room>();

        var updateRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101A",
            RoomType = "Suite",
            PricePerNight = 200.00m,
            Capacity = 3
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/room/{createdRoom!.Id}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var updatedRoom = await response.Content.ReadFromJsonAsync<Room>();
        Assert.That(updatedRoom, Is.Not.Null);
        Assert.That(updatedRoom!.RoomNumber, Is.EqualTo(updateRequest.RoomNumber));
        Assert.That(updatedRoom.RoomType, Is.EqualTo(updateRequest.RoomType));
        Assert.That(updatedRoom.PricePerNight, Is.EqualTo(updateRequest.PricePerNight));
        Assert.That(updatedRoom.Capacity, Is.EqualTo(updateRequest.Capacity));
    }

    [Test]
    public async Task UpdateRoom_WithNonExistingRoom_ReturnsNotFound()
    {
        // Arrange
        var nonExistingRoomId = 999;
        var updateRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/room/{nonExistingRoomId}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteRoom_WithExistingRoom_ReturnsNoContent()
    {
        // Arrange
        var createRequest = new RoomRequest
        {
            HotelId = 1,
            RoomNumber = "101",
            RoomType = "Deluxe",
            PricePerNight = 150.00m,
            Capacity = 2
        };
        var createResponse = await _client.PostAsJsonAsync("/api/room", createRequest);
        var createdRoom = await createResponse.Content.ReadFromJsonAsync<Room>();

        // Act
        var response = await _client.DeleteAsync($"/api/room/{createdRoom!.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify room is deleted
        var getResponse = await _client.GetAsync($"/api/room/{createdRoom.Id}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteRoom_WithNonExistingRoom_ReturnsNotFound()
    {
        // Arrange
        var nonExistingRoomId = 999;

        // Act
        var response = await _client.DeleteAsync($"/api/room/{nonExistingRoomId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}

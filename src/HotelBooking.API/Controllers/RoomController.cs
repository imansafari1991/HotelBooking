using RoomManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly RoomService _roomService;

    public RoomController(RoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] RoomRequest request)
    {
        try
        {
            var room = await _roomService.CreateRoom(request);
            return CreatedAtRoute(nameof(GetRoom), new { id = room.Id }, room);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}", Name = nameof(GetRoom))]
    public async Task<IActionResult> GetRoom([FromRoute] int id)
    {
        try
        {
            var room = await _roomService.GetRoomAsync(id);
            return Ok(room);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<IActionResult> GetRoomsByHotel([FromRoute] int hotelId)
    {
        var rooms = await _roomService.GetRoomsByHotelAsync(hotelId);
        return Ok(rooms);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom([FromRoute] int id, [FromBody] RoomRequest request)
    {
        try
        {
            var room = await _roomService.UpdateRoom(id, request);
            return Ok(room);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom([FromRoute] int id)
    {
        try
        {
            await _roomService.DeleteRoom(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

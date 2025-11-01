using BookingManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        try
        {
            var booking = await _bookingService.CreateBooking(request);
            return CreatedAtRoute(nameof(GetBooking), new { id = booking.Id }, booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }

    }
    [HttpGet("{id}", Name = nameof(GetBooking))]

    public async Task<IActionResult> GetBooking([FromRoute] int id, [FromQuery] int customerId)//Todo: should be resolved from user access token
    {

        try
        {
            var booking = await _bookingService.GetBookingAsync(id, customerId);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }

    }
}

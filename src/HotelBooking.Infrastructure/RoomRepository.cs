using HotelBooking.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace RoomManagement;

public class RoomRepository : IRoomRepository
{
    private readonly HotelBookingDbContext _context;

    public RoomRepository(HotelBookingDbContext context)
    {
        _context = context;
    }

    public Task<Room?> GetByIdAsync(int roomId)
    {
        return _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
    }

    public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId)
    {
        return await _context.Rooms.Where(r => r.HotelId == hotelId).ToListAsync();
    }

    public async Task SaveAsync(Room room)
    {
        _context.Add(room);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Room room)
    {
        _context.Update(room);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int roomId)
    {
        var room = await GetByIdAsync(roomId);
        if (room != null)
        {
            _context.Remove(room);
            await _context.SaveChangesAsync();
        }
    }
}

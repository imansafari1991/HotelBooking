using HotelBooking.Application.Interfaces;
using HotelBooking.Domain;
using HotelBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Stores;

public class RoomStore : IRoomStore
{
    private HotelBookingDbContext _context;

    public RoomStore(HotelBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (room == null)
        {
            throw new Exception("Room not found");
        }
        return room;
    }

    public async Task InsertAsync(Room room)
    {
        await _context.AddAsync(room);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExist(string roomNumber, CancellationToken cancellationToken)
    {
         return await _context.Rooms
            .AsNoTracking()
            .AnyAsync(r => r.RoomNumber == roomNumber, cancellationToken);
    }

    public async Task UpdateAsync(Room room)
    {
        _context.Update(room);
        await _context.SaveChangesAsync();
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomManagement;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(int roomId);
    Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId);
    Task SaveAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(int roomId);
}

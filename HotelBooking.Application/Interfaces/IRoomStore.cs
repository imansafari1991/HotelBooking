using HotelBooking.Domain;

namespace HotelBooking.Application.Interfaces;

public interface IRoomStore
{
    Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task InsertAsync(Room room);
    Task UpdateAsync(Room room);
    Task<bool> IsExist(string roomNumber, CancellationToken cancellationToken);
}
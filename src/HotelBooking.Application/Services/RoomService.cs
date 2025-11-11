namespace RoomManagement;

public class RoomService(IRoomRepository roomRepository)
{
    public async Task<Room> CreateRoom(RoomRequest roomRequest)
    {
        if (string.IsNullOrWhiteSpace(roomRequest.RoomNumber))
        {
            throw new ArgumentException(RoomErrorMessages.InvalidRoomNumber);
        }

        if (string.IsNullOrWhiteSpace(roomRequest.RoomType))
        {
            throw new ArgumentException(RoomErrorMessages.InvalidRoomType);
        }

        if (roomRequest.PricePerNight <= 0)
        {
            throw new ArgumentException(RoomErrorMessages.InvalidPricePerNight);
        }

        if (roomRequest.Capacity <= 0)
        {
            throw new ArgumentException(RoomErrorMessages.InvalidCapacity);
        }

        var room = new Room(
            roomRequest.HotelId,
            roomRequest.RoomNumber,
            roomRequest.RoomType,
            roomRequest.PricePerNight,
            roomRequest.Capacity);

        await roomRepository.SaveAsync(room);
        return room;
    }

    public async Task<Room> GetRoomAsync(int roomId)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            throw new KeyNotFoundException(string.Format(RoomErrorMessages.RoomNotFound, roomId));
        }
        return room;
    }

    public async Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId)
    {
        return await roomRepository.GetRoomsByHotelIdAsync(hotelId);
    }

    public async Task<Room> UpdateRoom(int roomId, RoomRequest roomRequest)
    {
        var existingRoom = await roomRepository.GetByIdAsync(roomId);
        if (existingRoom == null)
        {
            throw new KeyNotFoundException(string.Format(RoomErrorMessages.RoomNotFound, roomId));
        }

        if (string.IsNullOrWhiteSpace(roomRequest.RoomNumber))
        {
            throw new ArgumentException(RoomErrorMessages.InvalidRoomNumber);
        }

        if (string.IsNullOrWhiteSpace(roomRequest.RoomType))
        {
            throw new ArgumentException(RoomErrorMessages.InvalidRoomType);
        }

        if (roomRequest.PricePerNight <= 0)
        {
            throw new ArgumentException(RoomErrorMessages.InvalidPricePerNight);
        }

        if (roomRequest.Capacity <= 0)
        {
            throw new ArgumentException(RoomErrorMessages.InvalidCapacity);
        }

        var room = new Room(
            roomRequest.HotelId,
            roomRequest.RoomNumber,
            roomRequest.RoomType,
            roomRequest.PricePerNight,
            roomRequest.Capacity)
        {
            Id = roomId
        };

        await roomRepository.UpdateAsync(room);
        return room;
    }

    public async Task DeleteRoom(int roomId)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            throw new KeyNotFoundException(string.Format(RoomErrorMessages.RoomNotFound, roomId));
        }

        await roomRepository.DeleteAsync(roomId);
    }
}

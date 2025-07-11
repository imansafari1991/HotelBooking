using HotelBooking.Application.Interfaces;
using HotelBooking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application
{
    public class RoomService
    {
        private IRoomStore _roomStore;

        public RoomService(IRoomStore roomStore)
        {
            _roomStore = roomStore;
        }

        public async Task CreateNewRoomAsync(string roomNumber, string countryCode, decimal price,CancellationToken cancellationToken)
        {
            Room room = Room.Create(roomNumber, countryCode, price);
            if (await _roomStore.IsExist(roomNumber, cancellationToken))
            {
                throw new Exception("Room with this number already exists");
            }
            await _roomStore.InsertAsync(room);
        }
    }
}

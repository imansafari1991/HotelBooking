using System;
using System.Collections.Generic;
using System.Text;

namespace RoomManagement;

public class RoomErrorMessages
{
    public const string InvalidRoomNumber = "Room number cannot be empty.";
    public const string InvalidRoomType = "Room type cannot be empty.";
    public const string InvalidPricePerNight = "Price per night must be greater than zero.";
    public const string InvalidCapacity = "Capacity must be greater than zero.";
    public const string RoomNotFound = "Room with ID {0} not found.";
    public const string RoomNumberAlreadyExists = "Room number already exists for this hotel.";
}

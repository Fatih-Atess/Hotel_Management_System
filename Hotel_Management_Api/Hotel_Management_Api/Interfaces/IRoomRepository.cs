using Hotel_Management_Api.DTOs;
using Hotel_Management_Api.Models;

namespace Hotel_Management_Api.Interfaces
{
   
        public interface IRoomRepository
        {
            Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
            Task<IEnumerable<Room>> GetAllRoomsAsync();
            Task<bool> AddRoomAsync(RoomDto room);
        }
    
}

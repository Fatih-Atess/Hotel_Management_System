using Hotel_Management_Api.Models;
using Hotel_Management_Api.DTOs;


namespace Hotel_Management_Api.Interfaces
{
    public interface IReservationRepository
    {
        Task<bool> CreateReservationAsync(CreateReservationRequest request);
        Task<bool> DeleteReservationAsync(int id);
        //Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<IEnumerable<ReservationRoomDto>> GetAllReservationsWithRoomsAsync();
    }
}

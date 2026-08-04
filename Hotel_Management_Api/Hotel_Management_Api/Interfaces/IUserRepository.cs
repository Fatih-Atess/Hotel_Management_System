using Hotel_Management_Api.DTOs;

namespace Hotel_Management_Api.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddUserAsync(UserDto user);
    }
}

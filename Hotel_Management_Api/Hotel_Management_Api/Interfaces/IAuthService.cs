using Hotel_Management_Api.DTOs;

namespace Hotel_Management_Api.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> AuthenticateAsync(string username, string password);
    }
}


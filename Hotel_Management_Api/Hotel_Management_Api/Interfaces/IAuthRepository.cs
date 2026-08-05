namespace Hotel_Management_Api.Interfaces
{
    public interface IAuthRepository
    {
        Task<(string Rol, string PasswordHash)?> GetUserCredentialsAsync(string username);
    }
}
 
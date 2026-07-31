namespace Hotel_Management_Api.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> GetRoleAsync(string username, string password);
    }
}

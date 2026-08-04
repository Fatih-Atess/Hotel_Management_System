using Hotel_Management_Api.Interfaces;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Hotel_Management_Api.Repositories
{
    public class AuthRepository :IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration) 
        {
         _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<string> GetRoleAsync(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT Rol FROM user WHERE Kullanici_Adi = @Username AND Sifre = @Password";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetString("Rol");
                        }
                    }
                }
            }
            return null;
        }
    }
}

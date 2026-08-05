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

        public async Task<(string Rol, string PasswordHash)?> GetUserCredentialsAsync(string username)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT Rol, Sifre FROM user WHERE Kullanici_Adi = @Username";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                             string rol = reader.GetString("Rol");
                            string passwordHash = reader.GetString("Sifre");

                            return (rol, passwordHash);
                        }
                    }
                }
            }
            return null;
        }
    }
}

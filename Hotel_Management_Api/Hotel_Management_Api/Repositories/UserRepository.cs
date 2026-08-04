using Hotel_Management_Api.DTOs;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using MySql.Data.MySqlClient;


namespace Hotel_Management_Api.Repositories
{
    public class UserRepository: IUserRepository
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> AddUserAsync(UserDto user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string doesUserExist = "SELECT COUNT(*) FROM user WHERE Kullanici_Adi = @Kullanici_Adi";


                using (var command = new MySqlCommand(doesUserExist, connection))
                {
                    command.Parameters.AddWithValue("@Kullanici_Adi", user.Kullanici_Adi);
                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        return false;
                    }
                }

                string sql = "INSERT INTO user (Kullanici_Adi, Sifre) VALUES (@Kullanici_Adi, @Sifre)";

                using (var cmd = new MySqlCommand(sql, connection))
                {

                    cmd.Parameters.AddWithValue("@Kullanici_Adi", user.Kullanici_Adi);
                    cmd.Parameters.AddWithValue("@Sifre", user.Sifre);
                    

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }       
            }
        }
    }
}

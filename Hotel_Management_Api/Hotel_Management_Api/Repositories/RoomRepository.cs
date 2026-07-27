using Hotel_Management_Api.DTOs;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Hotel_Management_Api.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        // IConfiguration allows us to read the connection string from secrets.json / appsettings.json.
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public RoomRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            var rooms = new List<Room>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM room";

                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rooms.Add(new Room
                            {
                                ID = reader.GetInt32("ID"),
                                Oda_Numarasi = reader.GetString("Oda_Numarasi"),
                                Tip = reader.GetString("Tip"),
                                Gecelik_Fiyat = reader.GetDecimal("Gecelik_Fiyat")
                            });
                        }
                    }
                }
            }
            return rooms;
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var availableRooms = new List<Room>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"
                    SELECT r.ID, r.Oda_Numarasi, r.Tip, r.Gecelik_Fiyat
                    FROM Room r
                    WHERE r.ID NOT IN (
                        SELECT res.Oda_ID
                        FROM Reservation res
                        WHERE res.Giris_Tarihi < @CheckOut AND res.Cikis_Tarihi > @CheckIn
                    )";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CheckIn", checkIn);
                    command.Parameters.AddWithValue("@CheckOut", checkOut);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            availableRooms.Add(new Room
                            {
                                ID = reader.GetInt32("ID"),
                                Oda_Numarasi = reader.GetString("Oda_Numarasi"),
                                Tip = reader.GetString("Tip"),
                                Gecelik_Fiyat = reader.GetDecimal("Gecelik_Fiyat")
                            });
                        }
                    }
                }
            }
            return availableRooms;
        }

        public async Task<bool> AddRoomAsync(RoomDto room)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string doesRoomExist = "SELECT COUNT(*) FROM room WHERE Oda_Numarasi = @Oda_Numarasi";

                
                using (var command = new MySqlCommand(doesRoomExist, connection))
                {
                    command.Parameters.AddWithValue("@Oda_Numarasi", room.Oda_Numarasi);
                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        return false;
                    }
                }

                string sql = "INSERT INTO room (Oda_Numarasi, Tip, Gecelik_Fiyat) VALUES (@Oda_Numarasi, @Tip, @Gecelik_Fiyat)";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                   
                    cmd.Parameters.AddWithValue("@Oda_Numarasi", room.Oda_Numarasi);
                    cmd.Parameters.AddWithValue("@Tip", room.Tip);
                    cmd.Parameters.AddWithValue("@Gecelik_Fiyat", room.Gecelik_Fiyat);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> RemoveRoomAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM room WHERE ID = @ID";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }
    }
}
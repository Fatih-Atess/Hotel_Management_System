using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Hotel_Management_Api.DTOs;

namespace Hotel_Management_Api.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        //IConfiguration allows us to read the connection string from secrets.json / appsettings.json.
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
                            rooms.Add(
                            new Room
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

    }
}

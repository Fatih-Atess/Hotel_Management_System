using Hotel_Management_Api.DTOs;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;

namespace Hotel_Management_Api.Repositories
{
    public class ReservationRepository: IReservationRepository
    {
        private readonly string _connectionString;

        public ReservationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CreateReservationAsync(CreateReservationRequest request)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string checkOverlapQuery = @"SELECT COUNT(1) FROM reservation
                    WHERE Oda_ID = @Oda_ID 
                    AND (Giris_Tarihi < @Cikis_Tarihi 
                    AND Cikis_Tarihi > @Giris_Tarihi)";
                using (var overlapCommand = new MySqlCommand(checkOverlapQuery, connection))
                {
                    overlapCommand.Parameters.AddWithValue("@Oda_ID", request.Oda_ID);
                    overlapCommand.Parameters.AddWithValue("@Giris_Tarihi", request.Giris_Tarihi);
                    overlapCommand.Parameters.AddWithValue("@Cikis_Tarihi", request.Cikis_Tarihi);

                    var count = Convert.ToInt32(await overlapCommand.ExecuteScalarAsync());
                    if(count > 0)
                    {
                        return false;
                    }
                }

                decimal nightPrice = 0;
                string getPriceQuery = "SELECT Gecelik_Fiyat FROM room WHERE ID = @Oda_ID";

                using (var priceCommand = new MySqlCommand(getPriceQuery, connection))
                {
                    priceCommand.Parameters.AddWithValue("@Oda_ID", request.Oda_ID);
                    var result = await priceCommand.ExecuteScalarAsync();

                    if(result == null)
                    {
                        return false;
                    }

                    nightPrice = Convert.ToDecimal(result);
                }

                int days = (request.Cikis_Tarihi - request.Giris_Tarihi).Days;
                decimal toplamUcret = days * nightPrice;

                string sql = @"INSERT INTO reservation 
                    (Oda_ID, Musteri_Ad_Soyad, Giris_Tarihi, Cikis_Tarihi, Toplam_Ucret) 
                    VALUES  (@Oda_ID, @Musteri_Ad_Soyad, @Giris_Tarihi, 
                    @Cikis_Tarihi, @Toplam_Ucret)";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Oda_ID", request.Oda_ID);
                    cmd.Parameters.AddWithValue("@Musteri_Ad_Soyad", request.Musteri_Ad_Soyad);
                    cmd.Parameters.AddWithValue("@Giris_Tarihi", request.Giris_Tarihi);
                    cmd.Parameters.AddWithValue("@Cikis_Tarihi", request.Cikis_Tarihi);
                    cmd.Parameters.AddWithValue("@Toplam_Ucret", toplamUcret);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }

            }
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = "DELETE FROM reservation WHERE ID = @ID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("ID", id);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        /*public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            var reservations = new List<Reservation>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM reservation";

                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reservations.Add(new Reservation
                            {
                                ID = reader.GetInt32("ID"),
                                Oda_ID = reader.GetInt32("Oda_ID"),
                                Musteri_Ad_Soyad = reader.GetString("Musteri_Ad_Soyad"),
                                Giris_Tarihi = reader.GetDateTime("Giris_Tarihi"),
                                Cikis_Tarihi = reader.GetDateTime("Cikis_Tarihi"),
                                Toplam_Ucret = reader.GetDecimal("Toplam_Ucret")
                            });
                        }
                    }
                }
            }
            return reservations;
        }*/

        public async Task<IEnumerable<ReservationRoomDto>> GetAllReservationsWithRoomsAsync()
        {
            var reservations = new List<ReservationRoomDto>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT res.ID, res.Oda_ID, r.Oda_Numarasi, r.Tip, res.Musteri_Ad_Soyad, res.Giris_Tarihi, res.Cikis_Tarihi, res.Toplam_Ucret FROM reservation res LEFT JOIN room r ON res.Oda_ID = r.ID;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reservations.Add(new ReservationRoomDto
                            {
                                ID = reader.GetInt32("ID"),
                                Oda_ID = reader.GetInt32("Oda_ID"),
                                Oda_Numarasi = reader.GetString("Oda_Numarasi"),
                                Tip = reader.GetString("Tip"),
                                Musteri_Ad_Soyad = reader.GetString("Musteri_Ad_Soyad"),
                                Giris_Tarihi = reader.GetDateTime("Giris_Tarihi"),
                                Cikis_Tarihi = reader.GetDateTime("Cikis_Tarihi"),
                                Toplam_Ucret = reader.GetDecimal("Toplam_Ucret")
                            });
                        }
                    }
                }
            }
            return reservations;
        }


    }
}

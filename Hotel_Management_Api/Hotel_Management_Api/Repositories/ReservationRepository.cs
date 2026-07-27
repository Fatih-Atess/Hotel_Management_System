using Hotel_Management_Api.DTOs;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using MySql.Data.MySqlClient;
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

        
    }
}

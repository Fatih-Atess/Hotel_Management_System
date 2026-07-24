namespace Hotel_Management_Api.DTOs
{
    public class CreateReservationRequest
    {
        public int Oda_ID { get; set; }
        public string Musteri_Ad_Soyad { get; set; }
        public DateTime Giris_Tarihi { get; set; }
        public DateTime Cikis_Tarihi { get; set; }
    }
}

namespace Hotel_Management_Api.Models
{
    public class Room
    {
        public int ID { get; set; }
        public string Oda_Numarasi { get; set; }
        public string Tip { get; set; }
        public decimal Gecelik_Fiyat { get; set; }
        public int Durum { get; set; }
    }
}

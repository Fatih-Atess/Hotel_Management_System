namespace Hotel_Management_Api.Models
{
	public class Reservation
	{
		public int Rezervasyon_ID { get; set; }
		public int Oda_ID { get; set; }
		public string Musteri_Ad_Soyad { get; set; }
		public DateTime Giris_Tarihi { get; set; }
		public DateTime Cikis_Tarihi { get; set; }
		public decimal Toplam_Ucret { get; set; }
	}
}

export interface Room {
    ID: number;
    oda_Numarasi: number;
    tip: string;
    gecelik_Fiyat: number;
}

export interface Reservation{
    ID?: number;
    oda_ID: number;
    musteri_Adi_Soyadi: string;
    giris_Tarihi: string;
    cikis_Tarihi: string;
    toplam_Ucret?: number;
}
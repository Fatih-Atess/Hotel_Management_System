export interface Room {
    id: number;
    oda_Numarasi: string;
    tip: string;
    gecelik_Fiyat: number;
    durum: number;
}

export interface UserDto {
    kullanici_Adi: string;
    sifre: string;
}


export interface Reservation {
    id?: number;
    oda_ID: number;
    musteri_Ad_Soyad: string;
    giris_Tarihi: string;
    cikis_Tarihi: string;
    toplam_Ucret?: number;
}

export interface ReservationRoom {
    id?: number;
    oda_ID: number;
    oda_Numarasi: string;
    tip: string;
    musteri_Ad_Soyad: string;
    giris_Tarihi: string;
    cikis_Tarihi: string;
    toplam_Ucret?: number;
}
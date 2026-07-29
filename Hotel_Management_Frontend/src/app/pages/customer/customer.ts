import { Component, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { Room, Reservation } from '../../models/models';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer.html',
  styleUrl: './customer.css',
})
export class Customer implements OnInit{
  checkinDate: string = '';
  checkoutDate: string = '';
  customerName: string = '';
  availableRooms: Room[] = [];

  minDateString: string = '';

  constructor(private apiService: Api){}

  ngOnInit(){
    const today = new Date();
    this.minDateString = today.toISOString().split('T')[0];
  }

  searchAvailableRooms() {
    if(!this.checkinDate || !this.checkoutDate){
      alert('Lütfen giriş ve çıkış tarihlerini seçiniz.');
      return;
    }

    const checkin = new Date(this.checkinDate);
    const checkout = new Date(this.checkoutDate);
    const today = new Date(this.minDateString);

    if(checkin < today || checkout < today){
      alert('Hata: Geçmiş bir tarihe rezervasyon yapılamaz.');
      this.availableRooms = [];
      return;
    }

    if(checkout <= checkin){
      alert('Hata: Çıkış tarihi, giriş tarihinden sonra olmalıdır.');
      this.availableRooms = [];
      return;
    }

    this.apiService.getAvailableRooms(this.checkinDate, this.checkoutDate)
    .subscribe({
      next: (rooms) => {
        this.availableRooms = rooms;
      },
      error: (err) => console.error('Oda araması sırasında hata oluştu:', err)
    });
  }

  bookRoom(room: Room){
    if(!this.customerName){
      alert('Lütfen rezervasyon için adınızı ve soyadınızı giriniz.');
      return;
    }

    const newReservation: Reservation = {
      oda_ID: room.id,
      musteri_Ad_Soyad: this.customerName,
      giris_Tarihi: this.checkinDate,
      cikis_Tarihi: this.checkoutDate
    };

    this.apiService.createReservation(newReservation)
    .subscribe({
      next: (res) => {
        this.customerName = '';
        this.searchAvailableRooms();
        alert('Rezervasyon başarıyla oluşturuldu!');
      },
      error: (err) => {
        alert('Rezervasyon yapılamadı.');
        console.error(err);
      }
    })
  }



}

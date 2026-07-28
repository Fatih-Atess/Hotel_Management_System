import { Component } from '@angular/core';
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
export class Customer {
  checkinDate: string = '';
  checkoutDate: string = '';
  customerName: string = '';
  availableRooms: Room[] = [];

  constructor(private apiService: Api){}

  searchAvailableRooms() {
    if(!this.checkinDate || !this.checkoutDate){
      alert('Lütfen giriş ve çıkış tarihlerini seçiniz.');
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
      oda_ID: room.ID,
      musteri_Ad_Soyad: this.customerName,
      giris_Tarihi: this.checkinDate,
      cikis_Tarihi: this.checkoutDate
    };

    this.apiService.createReservation(newReservation)
    .subscribe({
      next: (res) => {
        alert('Rezervasyon başarıyla oluşturuldu!');
        this.customerName = '';
        this.searchAvailableRooms();
      },
      error: (err) => {
        alert('Rezervasyon yapılamadı.');
        console.error(err);
      }
    })
  }



}

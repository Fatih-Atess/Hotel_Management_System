import { Component, OnInit, ChangeDetectorRef} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { Room, Reservation } from '../../models/models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})

export class Admin implements OnInit {
  allRooms: Room[] = [];
  allReservations: Reservation[] = [];
  availableRooms: Room[] = [];

  newRoom: Room = { id: 0, oda_Numarasi: '', tip: '', gecelik_Fiyat: 0};

  filterCheckin: string = '';
  filterCheckout: string = '';

  constructor(private apiService: Api, private cdr: ChangeDetectorRef){}

  ngOnInit(){
    this.loadAllRooms();
    this.loadAllReservations();
    console.log('Admin paneli yüklendi');
  }

  loadAllRooms(){
    this.apiService.getAllRooms().subscribe({
      next: (data) => {
        this.allRooms = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Odalar yüklenirken hata:', err)
    });
  }

  loadAllReservations(){
    this.apiService.getAllReservations().subscribe({
      next: (data) => {
        this.allReservations = data
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Rezervasyonlar yüklenirken hata:', err)
    });
  }

  findAvailableRooms() {
    if (!this.filterCheckin || !this.filterCheckout) {
      alert('Lütfen tarih seçiniz.');
      return;
    }
    this.apiService.getAvailableRooms(this.filterCheckin, this.filterCheckout).subscribe({
      next: (data) => this.availableRooms = data,
      error: (err) => console.error('Müsait odalar aranırken hata:', err)
    });
  }

  addNewRoom() {
    if (!this.newRoom.oda_Numarasi || !this.newRoom.tip || this.newRoom.gecelik_Fiyat <= 0) {
      alert('Lütfen geçerli oda bilgileri giriniz.');
      return;
    }

    this.apiService.addRoom(this.newRoom).subscribe({
      next: (response) => {
        this.loadAllRooms(); 
        this.newRoom = { id: 0, oda_Numarasi: '', tip: '', gecelik_Fiyat: 0 };
        alert('Oda sisteme başarıyla eklendi.');
      },
      error: (err) => {
        console.error('Oda eklenirken hata:', err)
      alert('Oda eklenemedi. Lütfen bağlantınızı kontrol edin.');
      }
    });
  }

  removeRoom(id: number) {
    if (confirm('Bu odayı silmek istediğinize emin misiniz? Üzerinde rezervasyon varsa hata alabilirsiniz.')) {
      this.apiService.deleteRoom(id).subscribe({
        next: () => {
          alert('Oda silindi.');
          this.loadAllRooms();
        },
        error: (err) => {
          alert('Hata: Bu odaya ait aktif bir rezervasyon bulunuyor olabilir.');
          console.error(err);
        }
      });
    }
  }

  removeReservation(id: number | undefined) {
    if (!id) return;
    if (confirm('Bu rezervasyonu iptal etmek istediğinize emin misiniz?')) {
      this.apiService.deleteReservation(id).subscribe({
        next: () => {
          alert('Rezervasyon başarıyla iptal edildi.');
          this.loadAllReservations();
        },
        error: (err) => console.error('Rezervasyon iptal edilirken hata:', err)
      });
    }
  }

}

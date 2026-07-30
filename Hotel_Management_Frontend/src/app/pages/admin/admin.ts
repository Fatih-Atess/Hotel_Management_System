import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { Room, Reservation, ReservationRoom } from '../../models/models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})

export class Admin implements OnInit {
  allRooms: Room[] = [];
  allReservations: ReservationRoom[] = [];
  availableRooms: Room[] = [];

  newRoom: Room = { id: 0, oda_Numarasi: '', tip: '', gecelik_Fiyat: 0, durum: 0 };

  checkinDate: string = '';
  checkoutDate: string = '';

  minDateString: string = '';

  isUpdateModalOpen: boolean = false;
  selectedRoomToUpdate: Room | null = null;

  errorMessage: string = '';

  openUpdateModal(room: Room) {
    if (room.durum === 1) return;
    this.selectedRoomToUpdate = { ...room };
    this.isUpdateModalOpen = true;
  }

  closeUpdateModal() {
    this.isUpdateModalOpen = false;
    this.selectedRoomToUpdate = null;
  }

  constructor(private apiService: Api, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.loadAllRooms();
    this.loadAllReservations();
    const today = new Date();
    this.minDateString = today.toISOString().split('T')[0];
    console.log('Admin paneli yüklendi');
  }

  loadAllRooms() {
    this.apiService.getAllRooms().subscribe({
      next: (data) => {
        this.allRooms = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Odalar yüklenirken hata:', err);
        this.errorMessage = err.error || err.message || 'Hata: Odalar yüklenemedi.';
      }
    });
  }

  loadAllReservations() {
    this.apiService.getAllReservationsWithRooms().subscribe({
      next: (data) => {
        this.allReservations = data
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Rezervasyonlar yüklenirken hata:', err);
        this.errorMessage = err.error || err.message || 'Hata: Rezervasyonlar yüklenemedi.';
      }
    });
  }

  searchAvailableRooms() {
    if (!this.checkinDate || !this.checkoutDate) {
      alert('Lütfen giriş ve çıkış tarihlerini seçiniz.');
      return;
    }

    const checkin = new Date(this.checkinDate);
    const checkout = new Date(this.checkoutDate);
    const today = new Date(this.minDateString);

    if (checkin < today || checkout < today) {
      alert('Hata: Geçmiş bir tarihe rezervasyon yapılamaz.');
      this.availableRooms = [];
      return;
    }

    if (checkout <= checkin) {
      alert('Hata: Çıkış tarihi, giriş tarihinden sonra olmalıdır.');
      this.availableRooms = [];
      return;
    }

    this.apiService.getAvailableRooms(this.checkinDate, this.checkoutDate)
      .subscribe({
        next: (rooms) => {
          this.availableRooms = rooms;
        },
        error: (err) => {
          console.error('Oda araması sırasında hata oluştu:', err);
          this.errorMessage = err.error || err.message || 'Hata: Oda araması başarısız.';
        }
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
        this.newRoom = { id: 0, oda_Numarasi: '', tip: '', gecelik_Fiyat: 0, durum: 0 };
        alert('Oda sisteme başarıyla eklendi.');
      },
      error: (err) => {
        console.error('Oda eklenirken hata:', err);
        this.errorMessage = err.error || err.message || 'Hata: Oda eklenemedi.';
      }
    });
  }

  removeRoom(id: number) {
    if (confirm('Bu odayı silmek istediğinize emin misiniz? Üzerinde rezervasyon varsa hata alabilirsiniz.')) {
      this.apiService.deleteRoom(id).subscribe({
        next: () => {
          alert('Oda silindi.');
          this.loadAllRooms();
          this.loadAllReservations();
        },
        error: (err) => {
          this.errorMessage = err.error || err.message || 'Hata: Oda silinemedi. Bu odaya ait aktif bir rezervasyon bulunuyor olabilir.';
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
        error: (err) => {
          this.errorMessage = err.error || err.message || 'Hata: Rezervasyon iptal edilemedi.';
          console.error(err);
        }

      });
    }
  }

  updateRoom() {
    if (!this.selectedRoomToUpdate) return;

    if (!this.selectedRoomToUpdate.oda_Numarasi || !this.selectedRoomToUpdate.tip || this.selectedRoomToUpdate.gecelik_Fiyat <= 0) {
      alert('Lütfen geçerli oda bilgileri giriniz.');
      return;
    }
    this.apiService.updateRoom(this.selectedRoomToUpdate.id, this.selectedRoomToUpdate).subscribe({
      next: () => {
        alert('Oda başarıyla güncellendi.');
        this.loadAllRooms();
        this.closeUpdateModal();
      },
      error: (err) => {
        this.errorMessage = err.error || err.message || 'Güncelleme başarısız. Lütfen verilerinizi kontrol ediniz.';
        console.error(err);
      }
    })
  }

}

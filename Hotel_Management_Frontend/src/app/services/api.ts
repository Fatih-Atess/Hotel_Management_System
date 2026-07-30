import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room, Reservation, ReservationRoom } from '../models/models';

@Injectable({
  providedIn: 'root',
})
export class Api {
  private baseUrl = 'http://localhost:5151';

  constructor(private http: HttpClient) { }

  getAllRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.baseUrl}/api/Rooms`);
  }
  getAvailableRooms(checkin: string, checkout: string): Observable<Room[]> {
    let params = new HttpParams()
      .set('checkIn', checkin)
      .set('checkOut', checkout);
    return this.http.get<Room[]>(`${this.baseUrl}/api/Rooms/available`, { params });
  }
  addRoom(room: Room): Observable<any> {
    return this.http.post(`${this.baseUrl}/api/Rooms`, room);
  }
  deleteRoom(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/api/Rooms/${id}`);
  }
  updateRoom(id: number, room: Room): Observable<any> {
    return this.http.put(`${this.baseUrl}/api/Rooms/${id}`, room);
  }
  getAllReservationsWithRooms(): Observable<ReservationRoom[]> {
    return this.http.get<ReservationRoom[]>(`${this.baseUrl}/api/Reservation`);
  }
  createReservation(reservation: Reservation): Observable<any> {
    return this.http.post(`${this.baseUrl}/api/Reservation`, reservation);
  }
  deleteReservation(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/api/Reservation/${id}`);
  }
}

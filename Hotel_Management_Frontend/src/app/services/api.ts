import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room, Reservation } from '../models/models';

@Injectable({
  providedIn: 'root',
})
export class Api {
  private baseUrl = 'http://localhost:5151';

  constructor(private http: HttpClient){}

  getAllRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.baseUrl}/Rooms`);
  }
  getAvailableRooms(checkin: string, checkout: string): Observable<Room[]>{
    let params = new HttpParams()
    .set('checkin', checkin)
    .set('checkout', checkout);
    return this.http.get<Room[]>(`${this.baseUrl}/Rooms`,{params});
  }
  addRoom(room: Room): Observable<any> {
    return this.http.post(`${this.baseUrl}/Rooms`, room);
  }
  deleteRoom(id:number):Observable<any> {
    return this.http.delete(`${this.baseUrl}/Rooms/${id}`);
  }
  getAllReservations(): Observable<Reservation[]>{
    return this.http.get<Reservation[]>(`${this.baseUrl}/Reservation`);
  }
  createReservation(reservation: Reservation): Observable<any> {
    return this.http.post(`${this.baseUrl}/Reservation`, reservation);
  }
  deleteReservation(id : number): Observable<any>{
    return this.http.delete(`${this.baseUrl}/Reservation/${id}`);
  }
}

import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AuthService} from './auth.service';
import {Meeting} from '../../pages/dashboard/event-dialog/event-dialog.component';
import {Availability} from '../../pages/dashboard/availability-dialog/availability-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private baseUrl = "https://localhost:44338"

  constructor(private http: HttpClient,
              private authService: AuthService) {
  }


  getUserProfile(): Observable<any> {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()});
    return this.http.get(this.baseUrl + "/user",
      {headers: reqHeader})
  }

  getAllUsers(): Observable<any> {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()});
    return this.http.get(this.baseUrl + "/getUsers",
      {headers: reqHeader})
  }

  getUserAvabilities(): Observable<any> {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()});
    return this.http.get(this.baseUrl + "/getAvailability",
      {headers: reqHeader})
  }

  getScheduleData(): Observable<any> {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()});
    return this.http.get(this.baseUrl + "/getSchedule",
      {headers: reqHeader})
  }

  postMeeting(meeting: Meeting): Observable<any> {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()});
    return this.http.post(this.baseUrl + "/addMeeting",
      meeting,
      {headers: reqHeader})
  }

  postAvailability(availability: Availability[]) {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()});
    return this.http.post(this.baseUrl + "/addAvailability",
      availability,
      {headers: reqHeader})
  }

  postStartProgram(): Observable<any> {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()});
    return this.http.post(this.baseUrl + "/startProgram",
      {headers: reqHeader})
  }

}

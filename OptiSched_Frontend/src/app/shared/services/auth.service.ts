import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {TOKEN_KEY} from '../constants/constants';
import {Router} from '@angular/router';
import {DOCUMENT} from '@angular/common';

export interface UserLogin {
  email: string,
  password: string
}

export interface UserRegistration extends UserLogin {
  name: string;
  surname: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:44338/api'
  private localStorage

  constructor(private httpClient: HttpClient,
              private router: Router,
              @Inject(DOCUMENT) private document: Document) {
    this.localStorage = document.defaultView?.localStorage;
  }

  public registerNewUser(formRegistrationData: UserRegistration): Observable<object> {
    return this.httpClient.post(`${this.baseUrl}/signup`, formRegistrationData)
  }

  public loginUser(formLoginData: UserLogin): Observable<object> {
    return this.httpClient.post(`${this.baseUrl}/signin`, formLoginData)
  }

  public isLoggedIn(): boolean {
    return !!this.getToken();
  }

  public logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.router.navigateByUrl('');
  }

  public getToken(): string | undefined | null {
    return this.localStorage?.getItem(TOKEN_KEY);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginRequest {
  email: string;
  username: string;
  password: string;
}

interface LoginResponse {
  token: string;
  expires: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}


  register(credentials: LoginRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Auth/register`, credentials);
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, credentials);
  }
}

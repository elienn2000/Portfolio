import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { DatePipe } from '@angular/common';

import { User } from '../models/user.model';

interface AuthRequest {
  email: string;
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private datePipe: DatePipe) {}


  register(credentials: AuthRequest): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/Auth/register`, credentials);
  }

  login(credentials: AuthRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Auth/login`, credentials);
  }

  sendVerificationEmail(user: User): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Auth/sendVerificationEmail`, user);
  }

  verifyActivationCode(email: string, code: string, ): Observable<void> {

    var payload = { 
      email: email, 
      verificationCode: code, 
      expiryTime: this.datePipe.transform(new Date(), 'yyyy-MM-ddTHH:mm:ss') || ''
    };

    return this.http.post<void>(`${this.apiUrl}/Auth/verifyActivationCode`, payload);
  }

  verifyEmail(token: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Auth/verifyMail`, { token });
  }
}

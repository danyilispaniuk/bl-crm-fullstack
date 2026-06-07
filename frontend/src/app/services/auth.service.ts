import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface LoginRequest {
  email?: string;
  password?: string;
}

export interface LoginResponse {
  token: string;
  user: {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
  };
}

export interface RegisterAdvisorRequest {
  email?: string;
  password?: string;
  firstName?: string;
  lastName?: string;
  personalId?: string | null;
  birthDate?: string;
  phoneNumber?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, credentials);
  }

  registerAdvisor(data: RegisterAdvisorRequest): Observable<{ message?: string; Message?: string }> {
    return this.http.post<{ message?: string; Message?: string }>(`${this.apiUrl}/auth/signup`, data);
  }
}

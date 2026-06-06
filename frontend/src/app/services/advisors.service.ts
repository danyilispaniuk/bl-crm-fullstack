import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Advisor {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  personalId?: string;
  birthDate?: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdvisorsService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  private platformId = inject(PLATFORM_ID);

  getAdvisors(): Observable<Advisor[]> {
    let role = null;
    if (isPlatformBrowser(this.platformId)) {
      role = localStorage.getItem('role');
    }
    
    if (role === 'Admin') {
      return this.http.get<Advisor[]>(`${this.apiUrl}/admin/advisor`);
    } else {
      return this.http.get<Advisor[]>(`${this.apiUrl}/advisor/lookup`);
    }
  }

  deleteAdvisor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/advisor/${id}`);
  }

  getAdvisor(id: string): Observable<Advisor> {
    return this.http.get<Advisor>(`${this.apiUrl}/advisor/${id}`);
  }
}

import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Client {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  personalId?: string;
  birthDate?: string;
  phoneNumber?: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClientsService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  private platformId = inject(PLATFORM_ID);

  getClients(): Observable<any[]> {
    let role = null;
    if (isPlatformBrowser(this.platformId)) {
      role = localStorage.getItem('role');
    }
    
    if (role === 'Admin') {
      return this.http.get<any[]>(`${this.apiUrl}/admin/client`);
    } else {
      return this.http.get<any[]>(`${this.apiUrl}/client/lookup`);
    }
  }

  deleteClient(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/client/${id}`);
  }

  getClient(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/client/${id}`);
  }

  createClient(clientData: any): Observable<Client> {
    return this.http.post<Client>(`${this.apiUrl}/client`, clientData);
  }
}

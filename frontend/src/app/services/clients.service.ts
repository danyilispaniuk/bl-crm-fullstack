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

export interface ClientLookup {
  id: string;
  name: string;
}

export interface CreateClientRequest {
  email: string;
  firstName: string;
  lastName: string;
  personalId?: string | null;
  birthDate: string;
  phoneNumber: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClientsService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  private platformId = inject(PLATFORM_ID);

  getClients(): Observable<Client[]> {
    let role = null;
    if (isPlatformBrowser(this.platformId)) {
      role = localStorage.getItem('role');
    }
    
    if (role === 'Admin') {
      return this.http.get<Client[]>(`${this.apiUrl}/admin/client`);
    } else {
      // Cast the lookup elements as Client since they are not used elsewhere in this version,
      // or define a separate endpoint when advisor client list is implemented.
      return this.http.get<Client[]>(`${this.apiUrl}/client/lookup`);
    }
  }

  deleteClient(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/client/${id}`);
  }

  getClient(id: string): Observable<Client> {
    return this.http.get<Client>(`${this.apiUrl}/client/${id}`);
  }

  createClient(clientData: CreateClientRequest): Observable<Client> {
    return this.http.post<Client>(`${this.apiUrl}/client`, clientData);
  }
}

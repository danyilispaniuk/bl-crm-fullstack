import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Contract {
  id: string;
  registrationNumber: string;
  institution: string;
  startDate: string;
  validityDate: string;
  endDate?: string;
  clientId: string;
  clientName: string;
  contractManagerId: string;
  contractManagerName: string;
}

@Injectable({
  providedIn: 'root'
})
export class ContractsService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  private platformId = inject(PLATFORM_ID);

  getContracts(): Observable<Contract[]> {
    return this.http.get<Contract[]>(`${this.apiUrl}/admin/contract`);
  }
}

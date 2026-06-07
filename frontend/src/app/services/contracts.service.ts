import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

import { Client } from './clients.service';

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
  participants?: Client[];
}

export interface CreateContractRequest {
  registrationNumber: string;
  institution: string;
  startDate: string;
  validityDate: string;
  endDate?: string | null;
  clientId: string;
  contractManagerId: string;
  participantIds: string[];
}

export interface AdvisorContractsResponse {
  managedContracts: Contract[];
  participatingContracts: Contract[];
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

  deleteContract(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/contract/${id}`);
  }

  getContract(id: string): Observable<Contract> {
    return this.http.get<Contract>(`${this.apiUrl}/contract/${id}`);
  }

  createContract(contractData: CreateContractRequest): Observable<Contract> {
    return this.http.post<Contract>(`${this.apiUrl}/contract`, contractData);
  }

  updateContract(id: string, contractData: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/contract/${id}`, contractData);
  }

  getAdvisorContracts(advisorId: string): Observable<AdvisorContractsResponse> {
    return this.http.get<AdvisorContractsResponse>(`${this.apiUrl}/advisor/${advisorId}/contracts`);
  }

  getClientContracts(clientId: string): Observable<Contract[]> {
    return this.http.get<Contract[]>(`${this.apiUrl}/client/${clientId}/contracts`);
  }

  exportContractsCsv(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/admin/contract/export/csv`, { responseType: 'blob' });
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExternalApiResultDto } from '../models/external-api-result';

@Injectable({
  providedIn: 'root'
})
export class ExternalApiService {
  // This route matches your modified DashboardController
  private readonly apiUrl = '/api/Dashboard';

  constructor(private http: HttpClient) { }

  getPopular(): Observable<ExternalApiResultDto[]> {
    return this.http.get<ExternalApiResultDto[]>(`${this.apiUrl}/popular`);
  }

  search(query: string): Observable<ExternalApiResultDto[]> {
    return this.http.get<ExternalApiResultDto[]>(`${this.apiUrl}/search`, { params: { query } });
  }
}

import { Injectable } from '@angular/core';
// FIX: Import from '@angular/common/http'
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
// FIX: Use lowercase 'dashboard' to match the filename
import { DashboardDto } from '../models/dashboard';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = '/api/Dashboard';

  constructor(private http: HttpClient) { }

  public getSummary(userId: string, filters: any): Observable<DashboardDto> {
    let params = new HttpParams();
    for (const key in filters) {
      if (filters[key] !== null && filters[key] !== '') {
        params = params.append(key, filters[key]);
      }
    }
    return this.http.get<DashboardDto>(`${this.apiUrl}/summary/${userId}`, { params });
  }
}

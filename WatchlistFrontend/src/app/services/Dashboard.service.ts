import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardDto } from '../models/Dashboard';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = '/api/Dashboard';

  constructor(private http: HttpClient) { }

  public getSummary(userId: string): Observable<DashboardDto> {

    return this.http.get<DashboardDto>(`${this.apiUrl}/summary/${userId}`);
  }
}

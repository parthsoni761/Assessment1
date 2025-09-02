import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WatchlistItemDto, CreateWatchlistItemDto, UpdateWatchlistItemDto } from '../models/watchlist-item';

@Injectable({
  providedIn: 'root'
})
export class WatchlistService {
  private readonly apiUrl = '/api/WatchListItems';

  constructor(private http: HttpClient) { }

  getItemsByUserId(
    userId: string,
    filters: any,
    sortColumn: string,
    sortDirection: string
  ): Observable<WatchlistItemDto[]> {
    let params = new HttpParams()
      .set('status', filters.status || '')
      .set('type', filters.type || '')
      .set('search', filters.search || '')
      .set('sortColumn', sortColumn || 'rating')
      .set('sortDirection', sortDirection || 'desc');

    return this.http.get<WatchlistItemDto[]>(`/api/WatchListItems/user/${userId}`, { params });
  }

  createItem(item: CreateWatchlistItemDto): Observable<WatchlistItemDto> {
    return this.http.post<WatchlistItemDto>(this.apiUrl, item);
  }

  updateItem(id: number, item: UpdateWatchlistItemDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, item);
  }

  deleteItem(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  // --- NEW METHOD for toggling IsFavorite ---
  toggleFavorite(id: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/toggleFavorite`, {});
  }
}

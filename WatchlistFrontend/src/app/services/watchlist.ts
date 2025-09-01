import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  WatchlistItemDto,
  CreateWatchlistItemDto,
  UpdateWatchlistItemDto
} from '../models/watchlist-item';

@Injectable({
  providedIn: 'root'
})
export class WatchlistService {
  // This URL exactly matches your controller's route attribute
  private readonly apiUrl = '/api/WatchListItems';

  constructor(private http: HttpClient) { }

  /**
   * Fetches all watchlist items for a specific user.
   * NOTE: Your backend GetByIdAsync(int id) method is actually filtering by UserId.
   * This corresponds to GET /api/WatchListItems/{userId}
   */
  getItemsByUserId(userId: string, filters: any): Observable<WatchlistItemDto[]> {
    let params = new HttpParams();
    for (const key in filters) {
      if (filters[key] !== null && filters[key] !== '') {
        params = params.append(key, filters[key]);
      }
    }
    // The route is now api/WatchListItems/user/{userId}
    return this.http.get<WatchlistItemDto[]>(`${this.apiUrl}/user/${userId}`, { params });
  }
  /**
   * Creates a new watchlist item.
   * Corresponds to POST /api/WatchListItems
   */
  createItem(item: CreateWatchlistItemDto): Observable<WatchlistItemDto> {
    return this.http.post<WatchlistItemDto>(this.apiUrl, item);
  }

  /**
   * Updates an existing watchlist item by its ID.
   * Corresponds to PUT /api/WatchListItems/{id}
   */
  updateItem(id: number, item: UpdateWatchlistItemDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, item);
  }

  /**
   * Deletes a watchlist item by its ID.
   * Corresponds to DELETE /api/WatchListItems/{id}
   */
  deleteItem(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}

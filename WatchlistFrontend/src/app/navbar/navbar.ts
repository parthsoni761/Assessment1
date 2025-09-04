import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../material.module';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';

// --- FIX: Update the import paths to use the new '.service.ts' filenames ---
import { AuthService } from '../services/auth.service';
import { ExternalApiService } from '../services/external-api.service';
import { WatchlistService } from '../services/watchlist.service';

import { ExternalApiResultDto } from '../models/external-api-result';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, MaterialModule, ReactiveFormsModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent implements OnInit {
  // ... The rest of the component logic is correct and does not need to change ...
  public username: string | null = null;
  public searchControl = new FormControl('');
  public searchResults: ExternalApiResultDto[] = [];
  public isSearchLoading = false;

  constructor(
    private authService: AuthService,
    private externalApiService: ExternalApiService,
    private watchlistService: WatchlistService
  ) {}

  ngOnInit(): void {
    this.username = this.authService.getUsername();
    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      tap(query => { if (query && query.length > 1) this.isSearchLoading = true; }),
      switchMap(query => {
        if (!query || query.length < 2) { this.searchResults = []; return []; }
        return this.externalApiService.search(query);
      })
    ).subscribe(results => {
      this.searchResults = results.slice(0, 7);
      this.isSearchLoading = false;
    });
  }

  addToWatchlist(item: ExternalApiResultDto): void {
    const userId = this.authService.getUserId();
    if (!userId) return;
    const newItem = { userId: Number(userId), title: item.title, itemType: item.itemType,
      releaseYear: item.releaseYear || new Date().getFullYear(), genre: item.genre,
      status: 'To Watch', rating: 0, isFavorite: false };
    this.watchlistService.createItem(newItem).subscribe(() => {
      alert(`'${item.title}' added to your watchlist!`);
      this.searchControl.setValue('');
    });
  }

  logout(): void {
    this.authService.logout();
  }
}

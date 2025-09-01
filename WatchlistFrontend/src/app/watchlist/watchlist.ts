import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';

import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

// Corrected import paths for robustness
import { WatchlistService } from '../services/watchlist';
import { AuthService } from '../services/auth';
import { WatchlistItemDto, CreateWatchlistItemDto } from '../models/watchlist-item';
import { MaterialModule } from '../material.module';
import { WatchlistDialogComponent } from '../watchlist-dialog/watchlist-dialog';

@Component({
  selector: 'app-watchlist',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule, MatSortModule],
  templateUrl: './watchlist.html',
  styleUrls: ['./watchlist.css']
})
export class WatchlistComponent implements OnInit, AfterViewInit {
  // --- THE FIX: Add 'progress' to this array to match the HTML ---
  public displayedColumns: string[] = ['isFavorite', 'title', 'itemType', 'genre', 'releaseYear', 'status', 'progress', 'rating', 'actions'];
  public dataSource = new MatTableDataSource<WatchlistItemDto>();
  public isLoading = true;
  public userId: string | null;

  @ViewChild(MatSort) sort!: MatSort;

  public filters = { status: '', type: '', search: '' };

  constructor(
    private watchlistService: WatchlistService,
    private authService: AuthService,
    public dialog: MatDialog
  ) {
    this.userId = this.authService.getUserId();
  }

  ngOnInit(): void {
    this.loadWatchlist();
  }

  ngAfterViewInit(): void {
    // This logic is correct for server-side sorting
    this.sort.sortChange.subscribe(() => {
      this.loadWatchlist();
    });
  }

  loadWatchlist(): void {
    if (!this.userId) return;

    this.isLoading = true;
    const sortColumn = this.sort?.active || 'rating';
    const sortDirection = this.sort?.direction || 'desc';

    this.watchlistService.getItemsByUserId(this.userId, this.filters, sortColumn, sortDirection).subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error("Failed to load watchlist", err);
        this.isLoading = false;
      }
    });
  }

  openAddModal(): void {
    const dialogRef = this.dialog.open(WatchlistDialogComponent, {
      width: '500px',
      data: {
        isEditing: false, item: {
          userId: Number(this.userId), title: '', itemType: 'Movie', genre: '',
          releaseYear: new Date().getFullYear(), status: 'To Watch', rating: 0, isFavorite: false,
          completedEpisodes: 0, totalEpisodes: 0
        } as CreateWatchlistItemDto
      }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.watchlistService.createItem(result).subscribe(() => this.loadWatchlist());
      }
    });
  }

  openEditModal(itemToEdit: WatchlistItemDto): void {
    const dialogRef = this.dialog.open(WatchlistDialogComponent, {
      width: '500px',
      data: { isEditing: true, item: itemToEdit }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.watchlistService.updateItem(itemToEdit.id, result).subscribe(() => this.loadWatchlist());
      }
    });
  }

  onToggleFavorite(item: WatchlistItemDto): void {
    item.isFavorite = !item.isFavorite;
    this.watchlistService.toggleFavorite(item.id).subscribe({
      error: (err) => {
        item.isFavorite = !item.isFavorite;
        console.error("Failed to update favorite status", err);
      }
    });
  }

  onDeleteItem(id: number): void {
    if (confirm("Are you sure?")) {
      this.watchlistService.deleteItem(id).subscribe(() => this.loadWatchlist());
    }
  }
}

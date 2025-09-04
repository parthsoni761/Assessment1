import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';

import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

// Corrected import paths for robustness
import { WatchlistService } from '../services/watchlist.service';
import { AuthService } from '../services/auth.service';
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
export class WatchlistComponent implements OnInit {
  // --- THE FIX: Add 'progress' to this array to match the HTML ---
  public displayedColumns: string[] = ['isFavorite', 'title', 'itemType', 'genre', 'releaseYear', 'status', 'progress', 'rating', 'actions'];
  public dataSource = new MatTableDataSource<WatchlistItemDto>();
  public isLoading = true;
  public userId: string | null;
  public sortState = {
  active: '',
  direction: 'desc' as 'asc' | 'desc'
};

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
    // ✅ Update local state when user changes sort
    this.sort.sortChange.subscribe(sort => {
      this.sortState.active = sort.active;
      this.sortState.direction = sort.direction as 'asc' | 'desc';
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


  // --- NEW METHOD: Export to CSV ---
  exportToCsv(): void {
    const dataToExport = this.dataSource.data;
    if (!dataToExport || dataToExport.length === 0) {
      return;
    }

    // Define the headers for your CSV file
    const headers = ['Title', 'Type', 'Genre', 'Year', 'Status', 'Rating (1-5)', 'Favorite', 'Completed Episodes', 'Total Episodes'];
    const csvRows = [headers.join(',')]; // Start with the header row

    // Helper function to safely handle fields that might contain commas
    const sanitizeField = (field: any): string => {
      const value = field === null || field === undefined ? '' : String(field);
      // If the value contains a comma, a quote, or a newline, enclose it in double quotes.
      if (value.includes(',') || value.includes('"') || value.includes('\n')) {
        // Also, any double quotes inside the value must be escaped by another double quote.
        return `"${value.replace(/"/g, '""')}"`;
      }
      return value;
    };

    // Iterate over the data to create each row
    for (const item of dataToExport) {
      const row = [
        sanitizeField(item.title),
        sanitizeField(item.itemType),
        sanitizeField(item.genre),
        sanitizeField(item.releaseYear),
        sanitizeField(item.status),
        sanitizeField(item.rating),
        sanitizeField(item.isFavorite ? 'Yes' : 'No'),
        sanitizeField(item.itemType === 'TV Show' ? item.completedEpisodes : ''),
        sanitizeField(item.itemType === 'TV Show' ? item.totalEpisodes : '')
      ].join(',');
      csvRows.push(row);
    }

    // Combine all rows into a single CSV string
    const csvContent = csvRows.join('\n');

    // Create a Blob to hold the CSV data
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });

    // Create a temporary link element to trigger the download
    const link = document.createElement('a');
    if (link.download !== undefined) {
      const url = URL.createObjectURL(blob);
      link.setAttribute('href', url);
      link.setAttribute('download', 'my-watchlist.csv');
      link.style.visibility = 'hidden';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
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

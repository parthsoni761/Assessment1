import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { WatchlistService } from '../services/watchlist';
import { AuthService } from '../services/auth';
import { WatchlistItemDto, CreateWatchlistItemDto, UpdateWatchlistItemDto } from '../models/watchlist-item';

@Component({
  selector: 'app-watchlist',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './watchlist.html',
  styleUrls: ['./watchlist.css']
})
export class WatchlistComponent implements OnInit {
  public watchlist: WatchlistItemDto[] = [];
  public isLoading = true;
  public userId: string | null;

  // State for the filter controls
  public filters = {
    status: '',
    genre: '',
    type: '',
    isFavorite: '',
    search: '',
    sortBy: ''
  };

  public isTitleSortAscending = true;

  // State for the Add/Edit modal
  public isModalVisible = false;
  public isEditing = false;
  public formModel!: CreateWatchlistItemDto | UpdateWatchlistItemDto;
  public editingItemId: number | null = null;

  constructor(
    private watchlistService: WatchlistService,
    private authService: AuthService
  ) {
    this.userId = this.authService.getUserId();
  }

  ngOnInit(): void {
    this.loadWatchlist();
  }



  toggleTitleSort(): void {
    // 1. Flip the direction
    this.isTitleSortAscending = !this.isTitleSortAscending;

    // 2. Set the sortBy filter based on the new direction
    this.filters.sortBy = this.isTitleSortAscending ? 'title_asc' : 'title_desc';

    // 3. Reload the data from the server
    this.loadWatchlist();
  }



  loadWatchlist(): void {
    if (!this.userId) {
      console.error("User is not logged in!");
      this.isLoading = false;
      return;
    }
    this.isLoading = true;
    this.watchlistService.getItemsByUserId(this.userId, this.filters).subscribe({
      next: (data) => {
        this.watchlist = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error("Failed to load watchlist", err);
        this.isLoading = false;
      }
    });
  }

  // --- Modal and Form Logic ---

  openAddModal(): void {
    this.isEditing = false;
    this.editingItemId = null;
    this.formModel = {
      userId: Number(this.userId),
      title: '',
      itemType: 'Movie',
      genre: '',
      releaseYear: new Date().getFullYear(),
      status: 'To Watch',
      rating: 0,
      isFavorite: false,
      completedEpisodes: 0,
      totalEpisodes: 0
    };
    this.isModalVisible = true;
  }

  openEditModal(item: WatchlistItemDto): void {
    this.isEditing = true;
    this.editingItemId = item.id;
    // Create a copy of the item to avoid mutating the table data directly
    this.formModel = { ...item };
    this.isModalVisible = true;
  }

  closeModal(): void {
    this.isModalVisible = false;
  }

saveItem(): void {
  if (!this.formModel) return;

  if (this.isEditing && this.editingItemId) {
    // Logic for updating an existing item
    this.watchlistService.updateItem(this.editingItemId, this.formModel as UpdateWatchlistItemDto).subscribe({
      next: () => {
        // --- THIS IS THE FIX ---
        // We must call our own method to reload the watchlist data.
        this.loadWatchlist();
        this.closeModal();
      },
      error: (err) => console.error("Failed to update item", err)
    });
  } else {
    // Logic for creating a new item
    this.watchlistService.createItem(this.formModel as CreateWatchlistItemDto).subscribe({
      next: () => {
        // --- THIS IS THE FIX ---
        // We must call our own method to reload the watchlist data.
        this.loadWatchlist();
        this.closeModal();
      },
      error: (err) => console.error("Failed to create item", err)
    });
  }
} 

  onDeleteItem(id: number): void {
    if (confirm("Are you sure you want to delete this item? This action cannot be undone.")) {
      this.watchlistService.deleteItem(id).subscribe({
        next: () => {
          // For a faster UI, remove the item from the local array instead of reloading
          this.watchlist = this.watchlist.filter(item => item.id !== id);
        },
        error: (err) => console.error("Failed to delete item", err)
      });
    }
  }
}

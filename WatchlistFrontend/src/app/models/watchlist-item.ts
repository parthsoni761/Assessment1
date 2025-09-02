// src/app/models/watchlist-item.ts

export interface WatchlistItemDto {
  id: number;
  userId: number;
  title: string;
  itemType: string;
  genre: string;
  releaseYear: number;
  status: string;
  rating: number;
  isFavorite: boolean;
  completedEpisodes?: number | null; // Optional properties
  totalEpisodes?: number | null;     // Optional properties
}

// For creating a new item
export interface CreateWatchlistItemDto {
  userId: number;
  title: string;
  itemType: string;
  genre: string;
  releaseYear: number;
  status: string;
  rating: number;
  isFavorite: boolean;
  completedEpisodes?: number | null;
  totalEpisodes?: number | null;
}

// For updating an existing item
export interface UpdateWatchlistItemDto {
  title: string;
  itemType: string;
  genre: string;
  releaseYear: number;
  status: string;
  rating: number;
  isFavorite: boolean;
  completedEpisodes?: number | null;
  totalEpisodes?: number | null;
}

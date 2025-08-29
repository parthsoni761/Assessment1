// src/app/models/dashboard.ts

export interface WatchListItemDto {
  id: number;
  userId: number;
  title: string;
  itemType: string;
  genre: string;
  releaseYear: number;
  status: string;
  rating: number;
  completedEpisodes: number;
  totalEpisodes: number;
  isFavorite: boolean;
}

export interface DashboardDto {
  totalItems: number;
  completedItems: number;
  pendingItems: number;
  favoriteItems: number;
  favoriteGenre: string;
  topRatedItems: WatchListItemDto[];
}

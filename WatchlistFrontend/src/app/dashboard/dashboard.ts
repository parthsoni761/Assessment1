import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { MaterialModule } from '../material.module';
import { AuthService } from '../services/auth.service';
import { DashboardService } from '../services/Dashboard.service';
import { DashboardDto } from '../models/Dashboard';

import { ExternalApiService } from '../services/external-api.service';
import { WatchlistService } from '../services/watchlist.service';
import { ExternalApiResultDto } from '../models/external-api-result';
import { CreateWatchlistItemDto } from '../models/watchlist-item';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, MaterialModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {

  public popularItems: ExternalApiResultDto[] = [];
  public isPopularLoading = true;
  public dashboardData: DashboardDto | null = null;
  public userId: string | null;
  public isLoading = true;

  // Doughnut Chart Properties
  public doughnutChartData: ChartConfiguration<'doughnut'>['data'] | null = null;
  public doughnutChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    cutout: '70%', // This creates the "doughnut" effect
  };

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService,
    private externalApiService: ExternalApiService, // <-- Inject new service
    private watchlistService: WatchlistService
  ) {
    this.userId = this.authService.getUserId();
  }

  ngOnInit(): void {
     this.fetchDashboardData();
     this.fetchPopularItems();
   }

     fetchPopularItems(): void {
    this.isPopularLoading = true;
    this.externalApiService.getPopular().subscribe(data => {
      this.popularItems = data;
      this.isPopularLoading = false;
    });
  }

  addToWatchlist(item: ExternalApiResultDto): void {
    if (!this.userId) return;

    const newItem: CreateWatchlistItemDto = {
      userId: Number(this.userId),
      title: item.title,
      itemType: item.itemType,
      releaseYear: item.releaseYear || new Date().getFullYear(),
      genre: item.genre, // Genre is now provided by the backend
      status: 'To Watch',
      rating: 0, // User can rate it after watching
      isFavorite: false,
    };

    this.watchlistService.createItem(newItem).subscribe(() => {
      alert(`'${item.title}' has been added to your watchlist!`);
      // Refresh the summary data to reflect the new total
      this.fetchDashboardData();
    });
  }

  fetchDashboardData(): void {
    if (!this.userId) { this.isLoading = false; return; }
    this.isLoading = true;
    this.dashboardService.getSummary(this.userId).subscribe({
      next: (data) => {
        this.dashboardData = data;
        this.updateChart();
        this.isLoading = false;
      },
      error: (err) => { console.error('Failed to fetch dashboard summary', err); this.isLoading = false; }
    });
  }

  updateChart(): void {
    if (!this.dashboardData) return;
    this.doughnutChartData = {
      labels: ['Completed', 'Pending/Watching'],
      datasets: [{
        data: [this.dashboardData.completedItems, this.dashboardData.pendingItems],
        backgroundColor: ['#3f51b5', '#ff4081'], // Primary and Accent theme colors
        hoverBackgroundColor: ['#303f9f', '#f50057'],
        borderWidth: 0,
      }],
    };
  }
}

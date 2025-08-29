import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartConfiguration } from 'chart.js';
// FIX: Removed the extra '}' at the end of this line

import { AuthService } from '../services/auth';
// FIX: Use lowercase 'dashboard.service' to match the renamed file
import { DashboardService } from '../services/Dashboard';
// FIX: Use lowercase 'dashboard' to match the renamed file
import { DashboardDto } from '../models/dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  // The rest of your component file was correct and needs no changes.
  public dashboardData: DashboardDto | null = null;
  public userId: string | null;
  public isLoading = true;

  public filters = { status: '', genre: '', type: '', isFavorite: '', search: '', sortBy: 'rating' };

  public barChartData: ChartConfiguration<'bar'>['data'] | null = null;
  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    scales: { y: { beginAtZero: true } }
  };

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService
  ) {
    this.userId = this.authService.getUserId();
  }

  ngOnInit(): void { this.fetchDashboardData(); }

  fetchDashboardData(): void {
    if (!this.userId) {
      console.error('User ID not found, cannot fetch data.');
      this.isLoading = false;
      return;
    }
    this.isLoading = true;
    this.dashboardService.getSummary(this.userId, this.filters).subscribe({
      next: (data) => {
        this.dashboardData = data;
        this.updateChart();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to fetch dashboard summary', err);
        this.isLoading = false;
      }
    });
  }

  updateChart(): void {
    if (!this.dashboardData) return;
    this.barChartData = {
      labels: ['Completed', 'Pending/Watching'],
      datasets: [
        {
          data: [this.dashboardData.completedItems, this.dashboardData.pendingItems],
          label: 'Watchlist Status',
          backgroundColor: ['#4CAF50', '#FFC107'],
          borderRadius: 5
        },
      ],
    };
  }

  logout(): void { this.authService.logout(); }
}

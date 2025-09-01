import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts'; // <-- Add this import for the chart

import { AuthService } from '../services/auth';
import { DashboardService } from '../services/Dashboard';
import { DashboardDto } from '../models/Dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule], // <-- Add BaseChartDirective here
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  public dashboardData: DashboardDto | null = null;
  public userId: string | null;
  public isLoading = true;

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

  ngOnInit(): void {
    this.fetchDashboardData();
  }

  fetchDashboardData(): void {
    if (!this.userId) {
      console.error('User ID not found, cannot fetch data.');
      this.isLoading = false;
      return;
    }
    this.isLoading = true;
    this.dashboardService.getSummary(this.userId).subscribe({
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
          // Corrected the typo
          data: [this.dashboardData.completedItems, this.dashboardData.pendingItems],
          label: 'Watchlist Status',
          backgroundColor: ['#4CAF50', '#FFC107'],
          borderRadius: 5
        },
      ],
    };
  }

  logout(): void {
    this.authService.logout();
  }
}

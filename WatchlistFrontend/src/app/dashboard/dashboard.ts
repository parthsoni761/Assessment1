import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { MaterialModule } from '../material.module';
import { AuthService } from '../services/auth';
import { DashboardService } from '../services/Dashboard';
import { DashboardDto } from '../models/Dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, MaterialModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
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

  constructor(private authService: AuthService, private dashboardService: DashboardService) {
    this.userId = this.authService.getUserId();
  }

  ngOnInit(): void { this.fetchDashboardData(); }

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

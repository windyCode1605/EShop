import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxEchartsModule, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts';
import { AdminDashboardService, DashboardResponseDto } from '../../services/admin-dashboard.service';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, NgxEchartsModule, ImageUrlPipe],
  providers: [provideEchartsCore({ echarts })],
  styleUrl: './admin-dashboard.component.scss',
  templateUrl: './admin-dashboard.component.html'
})
export class AdminDashboardComponent implements OnInit {

  isLoading = signal(true);
  data = signal<DashboardResponseDto | null>(null);
  chartOptions = signal<any>(null);

  constructor(private dashboardService: AdminDashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe({
      next: (res) => {
        this.data.set(res);
        this.initChart(res.revenueChart.map(p => p.label), res.revenueChart.map(p => p.revenue));
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  /** Format tiền tệ VNĐ */
  formatCurrency(value: number): string {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
  }

  /** Hiển thị % tăng trưởng với dấu + hoặc - */
  formatGrowth(value: number): string {
    return (value >= 0 ? '+' : '') + value.toFixed(1) + '%';
  }

  private initChart(labels: string[], revenues: number[]): void {
    this.chartOptions.set({
      tooltip: {
        trigger: 'axis',
        backgroundColor: '#18181B',
        textStyle: { color: '#FFFFFF' },
        borderWidth: 0,
        padding: [8, 12],
        axisPointer: { type: 'none' }
      },
      grid: { left: '0%', right: '0%', bottom: '0%', top: '10%', containLabel: true },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        data: labels,
        axisLine: { show: false },
        axisTick: { show: false },
        axisLabel: { color: '#71717A', margin: 16 }
      },
      yAxis: {
        type: 'value',
        splitLine: { lineStyle: { color: '#F4F4F5', type: 'dashed' } },
        axisLabel: { color: '#71717A' }
      },
      series: [{
        name: 'Doanh thu',
        type: 'line',
        smooth: true,
        symbol: 'none',
        data: revenues,
        lineStyle: { color: '#18181B', width: 3 },
        areaStyle: {
          color: {
            type: 'linear', x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              { offset: 0, color: 'rgba(24, 24, 27, 0.15)' },
              { offset: 1, color: 'rgba(24, 24, 27, 0)' }
            ]
          }
        }
      }]
    });
  }
}

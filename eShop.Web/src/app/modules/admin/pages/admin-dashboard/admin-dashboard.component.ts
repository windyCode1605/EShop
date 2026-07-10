import { Component, OnInit, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxEchartsModule, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, NgxEchartsModule],
  providers: [provideEchartsCore({ echarts })],
  styleUrl: './admin-dashboard.component.scss',
  templateUrl: './admin-dashboard.component.html'
})
export class AdminDashboardComponent implements OnInit {
  
  chartOptions: any;

  ngOnInit(): void {
    this.initChart();
  }

  initChart() {
    this.chartOptions = {
      tooltip: {
        trigger: 'axis',
        backgroundColor: '#18181B',
        textStyle: { color: '#FFFFFF' },
        borderWidth: 0,
        padding: [8, 12],
        axisPointer: { type: 'none' }
      },
      grid: {
        left: '0%',
        right: '0%',
        bottom: '0%',
        top: '10%',
        containLabel: true
      },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        data: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        axisLine: { show: false },
        axisTick: { show: false },
        axisLabel: { color: '#71717A', margin: 16 }
      },
      yAxis: {
        type: 'value',
        splitLine: {
          lineStyle: {
            color: '#F4F4F5',
            type: 'dashed'
          }
        },
        axisLabel: { color: '#71717A' }
      },
      series: [
        {
          name: 'Revenue',
          type: 'line',
          smooth: true,
          symbol: 'none',
          data: [12000, 15000, 11000, 18000, 22000, 19000, 28000, 25000, 31000, 29000, 36000, 45000],
          lineStyle: {
            color: '#18181B',
            width: 3
          },
          areaStyle: {
            color: {
              type: 'linear',
              x: 0, y: 0, x2: 0, y2: 1,
              colorStops: [{
                  offset: 0, color: 'rgba(24, 24, 27, 0.15)' // dark near top
              }, {
                  offset: 1, color: 'rgba(24, 24, 27, 0)' // transparent at bottom
              }]
            }
          }
        }
      ]
    };
  }
}

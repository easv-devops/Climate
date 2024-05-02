import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {
  ApexAnnotations,
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexMarkers,
  ApexStroke,
  ApexTitleSubtitle,
  ApexTooltip,
  ApexXAxis,
  ApexYAxis,
  ChartComponent
} from "ng-apexcharts";
//import {data} from "./series-data";
import {SensorDto} from "../../../models/Entities";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";


export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  markers: ApexMarkers;
  title: ApexTitleSubtitle;
  fill: ApexFill;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  annotations: ApexAnnotations;
  colors: any;
  toolbar: any;
};

@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.scss'],
})
export class GraphComponent implements OnInit {
  @Input() readings!: SensorDto[];
  @ViewChild("chart", {static: false}) chart!: ChartComponent;
  chartOptions: any = {};
  public activeOptionButton = "all";
  ws: WebSocketConnectionService;
  private unsubscribe$ = new Subject<void>();


  constructor(ws: WebSocketConnectionService) {
    this.ws = ws;
  }

  ngOnInit(): void {
    this.initChart();
    this.subscribeToTemperature(); // Showing temperature as default, since that's what is working now
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  initChart(): void {
    this.chartOptions = {
      series: [{data: []}],
      chart: {
        type: "area",
        height: 300
      },
      dataLabels: {
        enabled: false
      },
      markers: {
        size: 0
      },
      xaxis: {
        type: "datetime",
        tickAmount: 6
      },
      tooltip: {
        x: {
          format: "dd MMM yyyy"
        }
      },
      fill: {
        type: "gradient",
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.7,
          opacityTo: 0.9,
          stops: [0, 100]
        }
      }
    };
  }

  setTimeRange(range: string): void {
    const updateOptionsData: { [key: string]: { xaxis?: { min?: number; max?: number } } } = {
      "1d": {
        xaxis: {min: new Date().getTime() - (24 * 60 * 60 * 1000), max: new Date().getTime()}
      },
      "1m": {
        xaxis: {min: new Date().getTime() - (30 * 24 * 60 * 60 * 1000), max: new Date().getTime()}
      },
      "6m": {
        xaxis: {min: new Date().getTime() - (6 * 30 * 24 * 60 * 60 * 1000), max: new Date().getTime()}
      },
      "1y": {
        xaxis: {min: new Date().getTime() - (365 * 24 * 60 * 60 * 1000), max: new Date().getTime()}
      },
      "all": {
        xaxis: {min: undefined, max: undefined}
      }
    };

    this.chartOptions = {
      ...this.chartOptions,
      xaxis: {
        ...this.chartOptions.xaxis,
        ...(updateOptionsData[range]?.xaxis || {}) // Apply range updates or keep existing values
      }
    };
  }

  private subscribeToTemperature(): void {
    this.ws.tempReadings
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data: SensorDto[] | undefined) => {
        if (data && data.length > 0) {
          const newDataSeries = data.map((reading: SensorDto) => ({
            x: new Date(reading.TimeStamp).getTime(), // Convert timestamp to milliseconds
            y: reading.Value
          }));

          // Clear existing data before appending new data series
          if (!this.chartOptions.series[0] || !this.chartOptions.series[0].data) {
            this.chartOptions.series[0] = {data: []};
          } else {
            this.chartOptions.series[0].data = [];
          }

          // Append the new data series to the existing series
          this.chartOptions.series[0].data.push(...newDataSeries);

          // Update time range option
          this.setTimeRange(this.activeOptionButton);
        }
      });
  }

  updateGraph(option: string) {
    // Logic to update the graph based on the selected option
    // For example:
    if (option === 'temperature') {
      // Update graph for temperature
    } else if (option === 'humidity') {
      // Update graph for humidity
    } else if (option === 'pm') {
      // Update graph for PM
    }
  }
}

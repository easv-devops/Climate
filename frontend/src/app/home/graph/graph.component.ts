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
import {Observable, Subject, takeUntil} from "rxjs";
import {DeviceService} from "../devices/device.service";
import {ActivatedRoute} from "@angular/router";


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
  idFromRoute: number | undefined;
  @ViewChild("chart", {static: false}) chart!: ChartComponent;
  chartOptions: any = {};
  public activeOptionButton = "all";
  ws: WebSocketConnectionService;
  deviceService: DeviceService;
  activatedRoute: ActivatedRoute;
  private unsubscribe$ = new Subject<void>();

  constructor(ws: WebSocketConnectionService,
              deviceService: DeviceService,
              activatedRoute: ActivatedRoute,) {
    this.ws = ws;
    this.deviceService = deviceService;
    this.activatedRoute = activatedRoute;
  }

  ngOnInit(): void {
    this.getDeviceFromRoute();
    this.initChart();
    this.subscribeToReading(this.ws.temperatureReadings);
    this.subscribeToReading(this.ws.humidityReadings);
    this.updateGraph('temperature'); // Showing temperature as default, since that's what is working now
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  getDeviceFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
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
      yaxis: {
        labels: {
          formatter: function (value: number) {
            // Format the value as you desire, for example, to show only two decimal places
            return value.toFixed(1); // This will round the value to two decimal places
          }
        }
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

  /* Method to subscribe to the selected reading */
  /* Call by passing the observable as a parameter, like this: */
  /* this.subscribeToReading(this.ws.tempReadings); */
  private subscribeToReading(observable: Observable<SensorDto[] | undefined>): void {
    observable
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
    if (option === 'temperature') {
      // Update graph for temperature
      this.deviceService.getTemperatureByDeviceId(this.idFromRoute!);
    } else if (option === 'humidity') {
      // Update graph for humidity
      this.deviceService.getHumidityByDeviceId(this.idFromRoute!);
    } else if (option === 'pm') {
      // Update graph for PM
    }
  }
}

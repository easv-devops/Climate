import {Component, OnInit, ViewChild} from '@angular/core';
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

    const now: Date = new Date();
    const oneMonthAgo: Date = new Date(new Date().getTime() - 30 * 24 * 60 * 60 * 1000);

    // Request all the readings data
    this.deviceService.getTemperatureByDeviceId(this.idFromRoute!, oneMonthAgo, now);
    this.deviceService.getHumidityByDeviceId(this.idFromRoute!);
    this.deviceService.getPm25ByDeviceId(this.idFromRoute!);
    this.deviceService.getPm100ByDeviceId(this.idFromRoute!);

    this.updateGraph('temperature'); // Showing temperature as default
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
    //sets the standard historical range
    this.setTimeRange('1m')

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
  /* Call by passing the observable and series name as parameters, like this: */
  /* this.subscribeToReading(this.ws.temperatureReadings, 'Temperature') */
  subscribeToReadings(observable: Observable<Record<number, SensorDto[]> | undefined>, seriesName: string) {
    observable
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(readings => {
        if (readings) {

          // Find series to update
          let series = this.chartOptions.series.find((s: any) => s.name === seriesName);

          const data = readings[this.idFromRoute!];
          if (data && data.length > 0) {
            const newSeries = data.map((reading: SensorDto) => ({
              x: new Date(reading.TimeStamp).getTime(), // Convert timestamp to milliseconds
              y: reading.Value
            }));

            // Update the series with new data
            series = { name: seriesName, data: newSeries };
            this.chartOptions.series.push(series);

            // Update time range option
            this.setTimeRange(this.activeOptionButton);
          }
        }else {
        }
      });
  }

  updateGraph(option: string) {
    // Clear existing chart data & subscriptions
    this.chartOptions.series = [];
    this.ngOnDestroy();

    switch (option) {
      case 'temperature':
        this.subscribeToReadings(this.ws.temperatureReadings, 'Temperature')
        this.fetchDataFromLastTimestampToNow('Temperature');
        break;
      case 'humidity':
        this.subscribeToReadings(this.ws.humidityReadings, 'Humidity')
        this.fetchDataFromLastTimestampToNow('Humidity');
        break;
      case 'pm':
        this.subscribeToReadings(this.ws.pm25Readings, 'PM 2.5')
        this.subscribeToReadings(this.ws.pm100Readings, 'PM 10')
        this.fetchDataFromLastTimestampToNow('PM 2.5');
        this.fetchDataFromLastTimestampToNow('PM 10');
        break;
      case 'all':
        this.subscribeToReadings(this.ws.temperatureReadings, 'Temperature')
        this.subscribeToReadings(this.ws.humidityReadings, 'Humidity')
        this.subscribeToReadings(this.ws.pm25Readings, 'PM 2.5')
        this.subscribeToReadings(this.ws.pm100Readings, 'PM 10')

        this.fetchDataFromLastTimestampToNow('Temperature');
        this.fetchDataFromLastTimestampToNow('Humidity');
        this.fetchDataFromLastTimestampToNow('PM 2.5');
        this.fetchDataFromLastTimestampToNow('PM 10');

        break;
      default:
        console.error('Invalid option:', option);
    }
  }

  // Metode til at hente data fra det seneste tidspunkt i grafen og frem til nu
  fetchDataFromLastTimestampToNow(seriesName: string) {
    // Find den aktuelle serie baseret på navnet
    const series = this.chartOptions.series.find((s: any) => s.name === seriesName);

    if (series && series.data.length > 0) {
      // Find det seneste tidspunkt i serien
      const lastTimestamp = Math.max(...series.data.map((point: any) => point.x));

      // Opret starttidspunkt som det seneste tidspunkt i grafen
      const startTime = new Date(lastTimestamp + 10);

      // Opret sluttidspunkt som nuværende tidspunkt
      const endTime = new Date();

      // Hent data fra det seneste tidspunkt til nu
      switch (seriesName) {
        case 'Temperature':
          this.deviceService.getTemperatureByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
          /**
        case 'Humidity':
          this.deviceService.getHumidityByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
        case 'PM 2.5':
          this.deviceService.getPm25ByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
        case 'PM 10':
          this.deviceService.getPm100ByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
          */
        default:
          console.error('Invalid series name:', seriesName);
      }
    }
  }

}

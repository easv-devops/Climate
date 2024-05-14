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
} from "ng-apexcharts";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {DeviceService} from "../devices/device.service";
import {ActivatedRoute} from "@angular/router";
import {BaseGraphComponent} from "./graphSuper.component";


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
export class GraphComponent extends BaseGraphComponent implements OnInit {

  constructor(private ws: WebSocketConnectionService,
              private deviceService: DeviceService,
              private activatedRoute: ActivatedRoute,) {super()}

  ngOnInit(): void {
    this.getDeviceFromRoute();
    this.initChart();

    const now: Date = new Date();
    const oneDayAgo: Date = new Date(new Date().getTime() - 2 * 24 * 60 * 60 * 1000);
    const twoDayAgo: Date = new Date(new Date().getTime() - 1 * 24 * 60 * 60 * 1000);

    // Request all the readings data

    this.deviceService.getTemperatureByDeviceId(this.idFromRoute!, oneDayAgo, now);
    this.deviceService.getHumidityByDeviceId(this.idFromRoute!, oneDayAgo, now);
    this.deviceService.getPm25ByDeviceId(this.idFromRoute!, oneDayAgo, now);
    this.deviceService.getPm100ByDeviceId(this.idFromRoute!, oneDayAgo, now);

    this.updateGraph('temperature'); // Showing temperature as default
    // Update time range option
    this.setTimeRange("1d");
  }

  getDeviceFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }

  setTimeRange(range: string): void {
    const now = new Date().getTime();
    let minTime: number | undefined;
    let maxTime: number | undefined;

    this.activeOptionButton = range;

    switch (range) {
      case "1d":
        this.activeTimeInterval = 24 * 60 * 60 * 1000;
        minTime = now - (this.activeTimeInterval);
        maxTime = now;
        break;
      case "1m":
        this.activeTimeInterval = 30 * 24 * 60 * 60 * 1000;
        minTime = now - (this.activeTimeInterval);
        maxTime = now;
        break;
      case "6m":
        this.activeTimeInterval = 6 * 30 * 24 * 60 * 60 * 1000;
        minTime = now - (this.activeTimeInterval);
        maxTime = now;
        break;
      case "1y":
        this.activeTimeInterval = 365 * 24 * 60 * 60 * 1000;
        minTime = now - (this.activeTimeInterval);
        maxTime = now;
        break;
    }

    switch (this.currentReadingType) {
      case "temperature":
        this.fetchOlderReadingsIfNeeded("Temperature", new Date(minTime!));
        this.submitToHistory(this.ws.temperatureReadings, minTime!, maxTime!);
        break;

      case "humidity":
        this.fetchOlderReadingsIfNeeded("Humidity", new Date(minTime!));
        this.submitToHistory(this.ws.humidityReadings, minTime!, maxTime!);
        break;

      case "pm":
        this.fetchOlderReadingsIfNeeded("PM 2.5", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("PM 10", new Date(minTime!));
        this.submitToHistory(this.ws.pm25Readings, minTime!, maxTime!);
        this.submitToHistory(this.ws.pm100Readings, minTime!, maxTime!);
        break;

      case "all":
        this.fetchOlderReadingsIfNeeded("Temperature", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("Humidity", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("PM 2.5", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("PM 10", new Date(minTime!));

        this.submitToHistory(this.ws.temperatureReadings, minTime!, maxTime!);
        this.submitToHistory(this.ws.humidityReadings, minTime!, maxTime!);
        this.submitToHistory(this.ws.pm25Readings, minTime!, maxTime!);
        this.submitToHistory(this.ws.pm100Readings, minTime!, maxTime!);
        break;
    }
  }

  updateGraph(option: string) {
    // Clear existing chart data & subscriptions
    this.chartOptions.series = [];
    this.ngOnDestroy();
    this.currentReadingType = option

    switch (option) {
      case 'temperature':
        this.subscribeToReadings(this.ws.temperatureReadings, 'Temperature')
        this.fetchDataFromLastTimestampToNow('Temperature');//gets readings from last update to now and adds to the graph
        this.setTimeRange(this.activeOptionButton)
        break;
      case 'humidity':
        this.subscribeToReadings(this.ws.humidityReadings, 'Humidity')
        this.fetchDataFromLastTimestampToNow('Humidity');
        this.setTimeRange(this.activeOptionButton)
        break;
      case 'pm':
        this.subscribeToReadings(this.ws.pm25Readings, 'PM 2.5')
        this.subscribeToReadings(this.ws.pm100Readings, 'PM 10')
        this.fetchDataFromLastTimestampToNow('PM 2.5');
        this.fetchDataFromLastTimestampToNow('PM 10');
        this.setTimeRange(this.activeOptionButton)
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

    }
  }



  // Metode til at hente data fra det seneste tidspunkt i grafen og frem til nu
  fetchDataFromLastTimestampToNow(seriesName: string) {
    // Find den aktuelle serie baseret på navnet
    const series = this.chartOptions.series.find((s: any) => s.name === seriesName);


    if (series && series.data.length > 0) {

      const lastTimestamp = Math.max(...series.data.map((point: any) => point.x));
      const startTime = new Date(lastTimestamp);

      // Opret sluttidspunkt som nuværende tidspunkt
      const endTime = new Date();

      // Hent data fra det seneste tidspunkt til nu
      switch (seriesName) {
        case 'Temperature':
          this.deviceService.getTemperatureByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
        case 'Humidity':
          this.deviceService.getHumidityByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
        case 'PM 2.5':
          this.deviceService.getPm25ByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
        case 'PM 10':
          this.deviceService.getPm100ByDeviceId(this.idFromRoute!, startTime, endTime);
          break;
        default:
          console.error('Invalid series name:', seriesName);
          break;
      }
    }
  }

  fetchOlderReadingsIfNeeded(seriesName: string, startTime: Date) {
    // Find den aktuelle serie baseret på navnet
    const series = this.chartOptions.series.find((s: any) => s.name === seriesName);

    if (series && series.data.length > 0) {
      // Find det første tidspunkt i serien
      const firstTimestamp = Math.min(...series.data.map((point: any) => point.x));
      // Hvis starttidspunktet er tidligere end det første timestamp i listen
      if (startTime.getTime() < firstTimestamp) {
        // Hent data fra starttidspunktet til det første timestamp i listen
        switch (seriesName) {
          case 'Temperature':
            this.deviceService.getTemperatureByDeviceId(this.idFromRoute!, startTime, new Date(firstTimestamp));
            break;
          case 'Humidity':
            this.deviceService.getHumidityByDeviceId(this.idFromRoute!, startTime, new Date(firstTimestamp));
            break;
          case 'PM 2.5':
            this.deviceService.getPm25ByDeviceId(this.idFromRoute!, startTime, new Date(firstTimestamp));
            break;
          case 'PM 10':
            this.deviceService.getPm100ByDeviceId(this.idFromRoute!, startTime, new Date(firstTimestamp));
            break;
          default:
            console.error('Invalid series name:', seriesName);
            break;
        }
      }
    }
  }

}

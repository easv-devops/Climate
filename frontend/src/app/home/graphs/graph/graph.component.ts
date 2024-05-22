import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {WebSocketConnectionService} from '../../../web-socket-connection.service';
import {DeviceService} from '../../devices/device.service';
import {BaseGraphComponent} from "../graphSuper.component";
import {BreakpointObserver, Breakpoints} from "@angular/cdk/layout";


@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.scss'],
})
export class GraphComponent extends BaseGraphComponent implements OnInit {
  isMobile: boolean | undefined;

  constructor(private ws: WebSocketConnectionService,
              private deviceService: DeviceService,
              private activatedRoute: ActivatedRoute,
              private breakpointObserver: BreakpointObserver) {
    super();
    this.breakpointObserver.observe([
      Breakpoints.Handset, Breakpoints.Tablet
    ]).subscribe(result => {
      this.isMobile = result.matches;
    });
  }

  ngOnInit(): void {
    this.getDeviceFromRoute();
    this.initChart();

    this.updateGraph('temperature'); // Show temperature as default
    this.setTimeRange("1m");
  }

  getDeviceFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }

  setTimeRange(range: string): void {
    this.ngOnDestroy();
    const now = new Date().getTime();
    let minTime: number | undefined;
    let maxTime: number | undefined;

    this.activeOptionButton = range;

    switch (range) {
      case "1d":
        this.activeTimeInterval = 24 * 60 * 60 * 1000;
        minTime = now - this.activeTimeInterval;
        maxTime = now;
        break;
      case "1m":
        this.activeTimeInterval = 30 * 24 * 60 * 60 * 1000;
        minTime = now - this.activeTimeInterval;
        maxTime = now;
        break;
      case "6m":
        this.activeTimeInterval = 6 * 30 * 24 * 60 * 60 * 1000;
        minTime = now - this.activeTimeInterval;
        maxTime = now;
        break;
      case "1y":
        this.activeTimeInterval = 365 * 24 * 60 * 60 * 1000;
        minTime = now - this.activeTimeInterval;
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
    this.chartOptions.series = [];
    this.ngOnDestroy();
    this.currentReadingType = option;

    switch (option) {
      case 'temperature':
        this.subscribeToReadings(this.ws.temperatureReadings, 'Temperature');
        this.fetchDataFromLastTimestampToNow('Temperature');
        break;
      case 'humidity':
        this.subscribeToReadings(this.ws.humidityReadings, 'Humidity');
        this.fetchDataFromLastTimestampToNow('Humidity');
        break;
      case 'pm':
        this.subscribeToReadings(this.ws.pm25Readings, 'PM 2.5');
        this.subscribeToReadings(this.ws.pm100Readings, 'PM 10');
        this.fetchDataFromLastTimestampToNow('PM 2.5');
        this.fetchDataFromLastTimestampToNow('PM 10');
        break;
      case 'all':
        this.subscribeToReadings(this.ws.temperatureReadings, 'Temperature');
        this.subscribeToReadings(this.ws.humidityReadings, 'Humidity');
        this.subscribeToReadings(this.ws.pm25Readings, 'PM 2.5');
        this.subscribeToReadings(this.ws.pm100Readings, 'PM 10');
        this.fetchDataFromLastTimestampToNow('Temperature');
        this.fetchDataFromLastTimestampToNow('Humidity');
        this.fetchDataFromLastTimestampToNow('PM 2.5');
        this.fetchDataFromLastTimestampToNow('PM 10');
        break;
    }
    this.setTimeRange(this.activeOptionButton);
  }

  fetchDataFromLastTimestampToNow(seriesName: string) {
    const series = this.chartOptions.series.find((s: any) => s.name === seriesName);
    if (series && series.data.length > 0) {
      let lastTimestamp = Math.max(...series.data.map((point: any) => point.x));

      const startTime = new Date(lastTimestamp);
      const endTime = new Date();

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
    const series = this.chartOptions.series.find((s: any) => s.name === seriesName);
    let firstTimestamp;
    if (series) {
      firstTimestamp = Math.min(...series.data.map((point: any) => point.x));
    } else {
      firstTimestamp = new Date().getTime()
    }
    if (startTime.getTime() < firstTimestamp!) {
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


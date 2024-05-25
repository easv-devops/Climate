import {Component, OnInit} from '@angular/core';
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {ActivatedRoute} from "@angular/router";
import {BaseGraphComponent} from "../graphSuper.component";
import {RoomService} from "../../rooms/room.service";
import {BreakpointObserver, Breakpoints} from "@angular/cdk/layout";
import {LatestData, LatestReadingsDto} from "../../../../models/Entities";
import {ClientWantsToGetLatestDeviceReadingsDto} from "../../../../models/ClientWantsToGetLatestDeviceReadingsDto";
import {ClientWantsToGetLatestRoomReadingsDto} from "../../../../models/ClientWantsToGetLatestRoomReadingsDto";
import {takeUntil} from "rxjs";

@Component({
  selector: 'app-room-graph',
  templateUrl: '../graph/graph.component.html',
  styleUrls: ['../graph/graph.component.scss']
})
export class RoomGraphComponent extends BaseGraphComponent implements OnInit {
  isMobile: boolean | undefined;
  latestReadings: LatestData | undefined;
  isAvgReadings: boolean = true;

  constructor(private ws: WebSocketConnectionService,
              private roomService: RoomService,
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

    this.subscribeToLatestReadings();
    this.roomService.getLatestReadings(this.idFromRoute!);
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
        this.submitToHistory(this.ws.temperatureRoomReadings, minTime!, maxTime!);
        break;
      case "humidity":
        this.fetchOlderReadingsIfNeeded("Humidity", new Date(minTime!));
        this.submitToHistory(this.ws.humidityRoomReadings, minTime!, maxTime!);
        break;
      case "pm":
        this.fetchOlderReadingsIfNeeded("PM 2.5", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("PM 10", new Date(minTime!));
        this.submitToHistory(this.ws.pm25RoomReadings, minTime!, maxTime!);
        this.submitToHistory(this.ws.pm100RoomReadings, minTime!, maxTime!);
        break;
      case "all":
        this.fetchOlderReadingsIfNeeded("Temperature", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("Humidity", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("PM 2.5", new Date(minTime!));
        this.fetchOlderReadingsIfNeeded("PM 10", new Date(minTime!));

        this.submitToHistory(this.ws.temperatureRoomReadings, minTime!, maxTime!);
        this.submitToHistory(this.ws.humidityRoomReadings, minTime!, maxTime!);
        this.submitToHistory(this.ws.pm25RoomReadings, minTime!, maxTime!);
        this.submitToHistory(this.ws.pm100RoomReadings, minTime!, maxTime!);
        break;
    }
  }

  updateGraph(option: string) {
    this.chartOptions.series = [];
    this.ngOnDestroy();
    this.currentReadingType = option;

    switch (option) {
      case 'temperature':
        this.subscribeToReadings(this.ws.temperatureRoomReadings, 'Temperature');
        this.fetchDataFromLastTimestampToNow('Temperature');
        break;
      case 'humidity':
        this.subscribeToReadings(this.ws.humidityRoomReadings, 'Humidity');
        this.fetchDataFromLastTimestampToNow('Humidity');
        break;
      case 'pm':
        this.subscribeToReadings(this.ws.pm25RoomReadings, 'PM 2.5');
        this.subscribeToReadings(this.ws.pm100RoomReadings, 'PM 10');
        this.fetchDataFromLastTimestampToNow('PM 2.5');
        this.fetchDataFromLastTimestampToNow('PM 10');
        break;
      case 'all':
        this.subscribeToReadings(this.ws.temperatureRoomReadings, 'Temperature');
        this.subscribeToReadings(this.ws.humidityRoomReadings, 'Humidity');
        this.subscribeToReadings(this.ws.pm25RoomReadings, 'PM 2.5');
        this.subscribeToReadings(this.ws.pm100RoomReadings, 'PM 10');
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
      const endTime = new Date(new Date().getTime() + (2 * 60 * 60 * 1000)); // Add two hours for CEST

      switch (seriesName) {
        case 'Temperature':
          this.roomService.getTemperatureByRoomId(this.idFromRoute!, startTime, endTime);
          break;
        case 'Humidity':
          this.roomService.getHumidityByRoomId(this.idFromRoute!, startTime, endTime);
          break;
        case 'PM 2.5':
          this.roomService.getPm25ByRoomId(this.idFromRoute!, startTime, endTime);
          break;
        case 'PM 10':
          this.roomService.getPm100ByRoomId(this.idFromRoute!, startTime, endTime);
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
      firstTimestamp = new Date(new Date().getTime() + (2 * 60 * 60 * 1000)).getTime(); // Add two hours for CEST
    }
    if (startTime.getTime() < firstTimestamp!) {
      switch (seriesName) {
        case 'Temperature':
          this.roomService.getTemperatureByRoomId(this.idFromRoute!, startTime, new Date(firstTimestamp));
          break;
        case 'Humidity':
          this.roomService.getHumidityByRoomId(this.idFromRoute!, startTime, new Date(firstTimestamp));
          break;
        case 'PM 2.5':
          this.roomService.getPm25ByRoomId(this.idFromRoute!, startTime, new Date(firstTimestamp));
          break;
        case 'PM 10':
          this.roomService.getPm100ByRoomId(this.idFromRoute!, startTime, new Date(firstTimestamp));
          break;
        default:
          console.error('Invalid series name:', seriesName);
          break;
      }
    }
  }

  subscribeToLatestReadings() {
    this.ws.latestRoomReadings
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(readings => {
        if (readings) {
          this.latestReadings = readings[this.idFromRoute!];
        }
      })
  }
}

import {AlertDto} from "../../../models/Entities";

import { Component, OnInit } from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToGetAlertsDto} from "../../../models/ClientWantsToGetAlertsDto";
import {ClientWantsToEditAlertDto} from "../../../models/ClientWantsToEditAlertDto";

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})

export class AlertComponent implements OnInit {
  selectAllChecked: boolean = false;
  sortedAlerts: AlertDto[] = [];
  sortByOrder: { [key: string]: string } = {};
  showFilterDropdown: boolean = false;
  alertList?: AlertDto[];
  private unsubscribe$ = new Subject<void>();

  constructor(private ws: WebSocketConnectionService) {}

  ngOnInit() {
    this.getAlerts(false);
    this.subscribeToAlerts();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private subscribeToAlerts() {
    this.ws.alerts
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(alerts => {
        if (alerts) {
          this.alertList = alerts;
          this.sortedAlerts = [...this.alertList];
        }
      });
  }

  sortBy(column: keyof AlertDto) {
    // Skifter mellem sorteringsrækkefølgen stigende og faldende
    this.sortByOrder[column] = !this.sortByOrder[column] || this.sortByOrder[column] === 'asc' ? 'desc' : 'asc';

    // Nulstiller sorteringsrækkefølgen for andre kolonner
    Object.keys(this.sortByOrder).forEach(key => {
      if (key !== column) {
        this.sortByOrder[key] = '';
      }
    });

    // Sorter array af alerts ud fra valgt kolonne og sorteringsrækkefølgen
    this.sortedAlerts.sort((a, b) => {
      if (a[column] < b[column]) return this.sortByOrder[column] === 'asc' ? -1 : 1;
      if (a[column] > b[column]) return this.sortByOrder[column] === 'asc' ? 1 : -1;
      return 0;
    });
  }

  toggleSelectAll() {
    this.sortedAlerts.forEach(alert => {
      alert.IsRead = this.selectAllChecked;
    });
  }

  toggleFilterDropdown() {
    this.showFilterDropdown = !this.showFilterDropdown;
  }

  filterByIsRead(isRead: boolean | null) {
    if(this.alertList){
      if (isRead === null) {
        this.sortedAlerts = [...this.alertList]; // Ingen filter, vis alle alerts
      } else {
        this.sortedAlerts = this.alertList.filter(alert => alert.IsRead === isRead);
      }
      this.showFilterDropdown = false; // Skjul dropdown-menu efter filtrering
    }
  }

  onAlertReadChange(alert: any) {
    this.ws.socketConnection.sendDto(new ClientWantsToEditAlertDto({
      AlertId: alert.Id,
      DeviceId: alert.DeviceId,
      IsRead: alert.IsRead
    }))
  }

  getAlerts(b: boolean) {
    this.ws.socketConnection.sendDto(new ClientWantsToGetAlertsDto({
      IsRead: b
    }));
  }
}

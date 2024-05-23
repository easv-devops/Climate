import { AlertDto } from "../../../models/Entities";
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject, takeUntil } from "rxjs";
import { WebSocketConnectionService } from "../../web-socket-connection.service";
import { ClientWantsToGetAlertsDto } from "../../../models/ClientWantsToGetAlertsDto";
import { ClientWantsToEditAlertDto } from "../../../models/ClientWantsToEditAlertDto";

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})
export class AlertComponent implements OnInit, OnDestroy {
  selectAllChecked: boolean = false;
  sortedAlerts: AlertDto[] = [];
  sortByOrder: { [key: string]: string } = {};
  showFilterDropdown: boolean = false;
  alertList: AlertDto[] = [];
  isReadAlertsFetched: boolean = false;
  isUnreadAlertsFetched: boolean = false;
  currentFilter: boolean | null = null;
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
          this.applyFilter(this.currentFilter); // Apply the current filter whenever alerts are updated
        }
      });
  }

  private getAlerts(isRead: boolean) {
    if ((isRead && !this.isReadAlertsFetched) || (!isRead && !this.isUnreadAlertsFetched)) {
      this.ws.socketConnection.sendDto(new ClientWantsToGetAlertsDto({ IsRead: isRead }));
      if (isRead) {
        this.isReadAlertsFetched = true;
      } else {
        this.isUnreadAlertsFetched = true;
      }
    }
  }

  private applyFilter(isRead: boolean | null) {
    if (this.alertList) {
      if (isRead === null) {
        this.sortedAlerts = [...this.alertList]; // No filter, show all alerts
      } else {
        this.sortedAlerts = this.alertList.filter(alert => alert.IsRead === isRead);
      }
      this.showFilterDropdown = false; // Hide dropdown menu after filtering
    }
  }

  filterByIsRead(isRead: boolean | null) {
    this.currentFilter = isRead;

    // Fetch necessary alerts if they haven't been fetched already
    if (isRead === null) { // = "All"
      if (!this.isReadAlertsFetched) {
        this.getAlerts(true);
      }
      if (!this.isUnreadAlertsFetched) {
        this.getAlerts(false);
      }
    } else { // = Read or Unread
      this.getAlerts(isRead);
    }

    // Apply the filter to show the correct alerts based on the current filter
    this.applyFilter(isRead);
  }

  onAlertReadChange(alert: AlertDto) {
    this.ws.socketConnection.sendDto(new ClientWantsToEditAlertDto({
      AlertId: alert.Id,
      DeviceId: alert.DeviceId,
      IsRead: alert.IsRead
    }));

    // Optimistically update the local alert list
    const index = this.alertList.findIndex(a => a.Id === alert.Id);
    if (index !== -1) {
      this.alertList[index].IsRead = alert.IsRead;
    }

    // Reapply the current filter to update the view
    this.applyFilter(this.currentFilter);
  }

  sortBy(column: keyof AlertDto) {
    this.sortByOrder[column] = !this.sortByOrder[column] || this.sortByOrder[column] === 'asc' ? 'desc' : 'asc';

    Object.keys(this.sortByOrder).forEach(key => {
      if (key !== column) {
        this.sortByOrder[key] = '';
      }
    });

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
}

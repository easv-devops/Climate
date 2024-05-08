import {Component, OnInit} from '@angular/core';
import {Device, Room} from "../../models/Entities";
import {Subject, takeUntil} from "rxjs";
import {BreakpointObserver, Breakpoints} from "@angular/cdk/layout";
import {WebSocketConnectionService} from "../web-socket-connection.service";

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage implements OnInit {
  isMobile: boolean | undefined;
  private unsubscribe$ = new Subject<void>();
  roomMenuItem?: MenuItem;
  deviceMenuItem?: MenuItem;
  menuItems?: MenuItem[];

  constructor(private breakpointObserver: BreakpointObserver,
              private ws: WebSocketConnectionService) {
    this.breakpointObserver.observe([
      Breakpoints.Handset, Breakpoints.Tablet
    ]).subscribe(result => {
      this.isMobile = result.matches;
    });
  }

  ngOnInit() {
    this.loadMenu();
    this.subscribeToDevices();
    this.subscribeToRooms();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }


  loadRooms(rooms: Room[]) {

    //TODO: Load logged in user's rooms like for devices (max amount?).

    for (var r of rooms) {
      this.addSubItem(r.RoomName, 'rooms/' + r.Id, this.roomMenuItem!, 'chevron-forward')
    }
    this.addSubItem('All rooms', 'rooms/all', this.roomMenuItem!, 'grid')
    this.addSubItem('New room', 'rooms/add', this.roomMenuItem!, 'add')
  }

  loadDevices(devices: Device[]) {
    if (devices) {
      this.deviceMenuItem!.subItems = [];
      for (const d of devices) {
        this.addSubItem(d.DeviceName, 'devices/' + d.Id, this.deviceMenuItem!, 'chevron-forward');
      }
    }
  }

  subscribeToDevices() {
    this.ws.allDevices
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(devicesRecord => {
        if (devicesRecord !== undefined) {
          // Hent alle enheder fra recordet
          const devices = Object.values(devicesRecord);
          this.loadDevices(devices);
        }
      });
  }

  addSubItem(label: string, link: string, menuItem: MenuItem, icon: string) {
    var subItem: SubItem = {
      label: label, routerLink: link, icon: icon
    }
    menuItem.subItems?.push(subItem);
  }

  private loadMenu() {
    //Creates the Rooms accordion (MenuItem)
    this.roomMenuItem = {
      label: 'Rooms',
      icon: 'grid',
      subItems: []
    }

    //Creates the Devices accordion (MenuItem)
    this.deviceMenuItem = {
      label: 'Devices',
      icon: 'radio',
      subItems: []
    }

    //Adds the above MenuItems to the sidebar
    this.menuItems = [
      this.roomMenuItem,
      this.deviceMenuItem
    ];
  }

  private subscribeToRooms() {
    this.ws.allRooms
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(roomRecord => {
        if (roomRecord !== undefined) {
          // Hent alle enheder fra recordet
          const rooms = Object.values(roomRecord);
          this.clearMenuItems();
          this.loadRooms(rooms);
        }
      });
  }

  private clearMenuItems() {
    if (this.roomMenuItem) {
      this.roomMenuItem.subItems = [];
    }
  }
}

interface MenuItem {
  label: string;
  icon?: string;
  subItems?: SubItem[];
}

interface SubItem extends MenuItem {
  routerLink?: string;
}

import {Component, OnInit} from '@angular/core';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {WebSocketConnectionService} from "./web-socket-connection.service";
import {ClientWantsToGetDevicesByUserIdDto} from "../models/ClientWantsToGetDevicesByUserIdDto";
import {Device} from "../models/Entities";
import {Subject, takeUntil} from "rxjs";

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent implements OnInit {
  isMobile: boolean | undefined;
  allRooms: number[] = [1, 2, 3]; //allRooms: Room[] | undefined;
  allDevices?: Device[];
  private unsubscribe$ = new Subject<void>();
  authMenuItem?: MenuItem;
  roomMenuItem?: MenuItem;
  deviceMenuItem?: MenuItem;
  menuItems?: MenuItem[];

  constructor(private breakpointObserver: BreakpointObserver,
              private ws: WebSocketConnectionService) {
    this.breakpointObserver.observe([
      Breakpoints.HandsetPortrait
    ]).subscribe(result => {
      this.isMobile = result.matches;
    });
  }

  ngOnInit() {
    this.loadMenu();
    this.subscribeToDevices();
    this.loadRooms();
  }

  loadRooms() {
    //TODO: Load logged in user's rooms like for devices (max amount?).
    for (var r of this.allRooms) {
      this.addSubItem('Room ' + r.toString(), 'rooms/' + r.toString(), this.roomMenuItem!, 'chevron-forward')
    }
    this.addSubItem('All rooms', 'rooms/all', this.roomMenuItem!, 'grid')
    this.addSubItem('New room', 'rooms/new', this.roomMenuItem!, 'add')
  }

  loadDevices() {
    if (this.allDevices !== undefined) {
      this.deviceMenuItem!.subItems = []
      for (var d of this.allDevices) {
        this.addSubItem(d.DeviceName, 'devices/' + d.Id, this.deviceMenuItem!, 'chevron-forward')
      }
    }
  }

  subscribeToDevices() {
    this.ws.allDevices
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(devices => {
        if (devices !== undefined) {
          this.allDevices = devices;
          this.loadDevices()
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
    //Creates the Auth accordion (MenuItem) //TODO Delete eventually
    this.authMenuItem = {
      label: 'Auth eksempel',
      icon: 'person-circle',
      subItems: [
        {label: 'Login', routerLink: 'login', icon: 'log-in'},
        {label: 'Register', routerLink: 'register', icon: 'person-add'}
      ]
    }

    //Creates the Rooms accordion (MenuItem)
    this.roomMenuItem = {
      label: 'Rooms',
      icon: 'grid',
      subItems: []
    }

    //Creates the Devices accordion (MenuItem)
    this.deviceMenuItem = {
      label: 'Devices',
      icon: 'fitness',
      subItems: []
    }

    //Adds the above MenuItems to the sidebar
    this.menuItems = [
      this.authMenuItem,
      this.roomMenuItem,
      this.deviceMenuItem
    ];
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

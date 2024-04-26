import {Component, OnInit} from '@angular/core';
import {Device} from "../../models/Entities";
import {Subject, takeUntil} from "rxjs";
import {BreakpointObserver, Breakpoints} from "@angular/cdk/layout";
import {WebSocketConnectionService} from "../web-socket-connection.service";
import {B} from "@angular/cdk/keycodes";

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage implements OnInit {
  isMobile: boolean | undefined;
  allRooms: number[] = [1, 2, 3]; //allRooms: Room[] | undefined;
  allDevices?: Device[];
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
    this.loadRooms();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
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

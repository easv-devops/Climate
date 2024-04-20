import {Component, OnInit} from '@angular/core';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent implements OnInit {
  isMobile: boolean | undefined;
  menuItems: MenuItem[] = [
    {
      label: 'Auth eksempel',
      icon: 'person-circle',
      subItems: [
        {label: 'Login', routerLink: '/login'},
        {label: 'Register', routerLink: '/register'}
      ]
    }
  ];

  constructor(private breakpointObserver: BreakpointObserver) {
    this.breakpointObserver.observe([
      Breakpoints.HandsetPortrait
    ]).subscribe(result => {
      this.isMobile = result.matches;
    });
  }

  ngOnInit(): void {
    this.loadRooms();
    this.loadDevices();
    this.setSubItemIcon();
  }

  setSubItemIcon() {
    this.menuItems.forEach(menuItem => {
      if (menuItem.subItems) {
        menuItem.subItems.forEach(subItem => {
          subItem.icon = 'chevron-forward';
        });
      }
    });
  }

  loadRooms() {
    //TODO: Load logged in user's rooms (max amount?)
    var item: MenuItem[] = [{
      label: 'Rooms',
      icon: 'grid',
      subItems: [
        {label: 'Room 1', routerLink: '/home/page1'}, //{label: '{roomName}', routerLink: '/rooms/{roomId}'},
        {label: 'Room 2', routerLink: '/home/page1'}
      ]
    }]
    this.pushToMenuItems(item)
  }

  loadDevices() {
    //TODO: Load logged in user's devices (max amount?)
    var item: MenuItem[] = [{
      label: 'Devices',
      icon: 'fitness',
      subItems: [
        {label: 'Device 1', routerLink: '/home/page2'}, //{label: '{deviceName}', routerLink: '/rooms/{roomId}/{deviceId}'?}
        {label: 'Device 2', routerLink: '/home/page2'}
      ]
    }]
    this.pushToMenuItems(item)
  }

  pushToMenuItems(item: MenuItem[]) {
    item.forEach(i => this.menuItems.push(i))
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
